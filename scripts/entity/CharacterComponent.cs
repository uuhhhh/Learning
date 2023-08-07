using Godot;
using System;

public partial class CharacterComponent : CharacterBody2D {
	[Export] private float _speed;
	[Export] private float _jumpVelocity;
	[Export] private float _groundAcceleration;
	[Export] private float _airAcceleration;
	[Export] private float _groundDeceleration;
	[Export] private float _airDeceleration;
	[Export] private float _upwardsGravityScale;
	[Export] private float _downwardsGravityScale;
	[Export] private float _turnaroundAccelerationDampening;
	
	private float _gravity = ProjectSettings.GetSetting("physics/2d/default_gravity").AsSingle();
	
	private Timer _coyoteJumpTimer;
	private Timer _jumpBufferTimer;

	private float _currentInputSpeedScale;
	private float _intendedInputSpeedScale;
	private float _currentInputAccelerationModifier;
	private Tween _inputSpeedScaleTween;
	
	[Signal]
	public delegate void BecomeOnFloorEventHandler();

	[Signal]
	public delegate void BecomeOffFloorEventHandler();

	public override void _Ready() {
		_coyoteJumpTimer = GetNode<Timer>("CoyoteJumpTimer");
		_jumpBufferTimer = GetNode<Timer>("JumpBufferTimer");

		BecomeOnFloor += AttemptBufferedJump;
		BecomeOffFloor += HandleCoyoteJumpTimer;

		BecomeOnFloor += UpdateInputAcceleration;
		BecomeOffFloor += UpdateInputAcceleration;
	}

	public override void _PhysicsProcess(double delta) {
		Velocity = HandleInputVelocity(Velocity, delta);
		Velocity = HandleGravity(Velocity, delta);

		bool wasOnFloor = IsOnFloor();
		MoveAndSlide();
		CheckFloorStatusChange(wasOnFloor);
	}

	private Vector2 HandleInputVelocity(Vector2 velocity, double delta) {
		Vector2 newVelocity = velocity;
		
		newVelocity.X = _currentInputSpeedScale * _speed;

		return newVelocity;
	}

	private Vector2 HandleGravity(Vector2 velocity, double delta) {
		Vector2 newVelocity = velocity;
		
		if (!IsOnFloor()) {
			float gravityScale = newVelocity.Y > 0 ? _downwardsGravityScale : _upwardsGravityScale;
			newVelocity.Y += _gravity * gravityScale * (float)delta;
		}

		return newVelocity;
	}

	private void CheckFloorStatusChange(bool wasOnFloor) {
		if (wasOnFloor && !IsOnFloor()) {
			EmitSignal(SignalName.BecomeOffFloor);
		} else if (!wasOnFloor && IsOnFloor()) {
			EmitSignal(SignalName.BecomeOnFloor);
		}
	}
	
	// -- Jumping methods --

	private void HandleCoyoteJumpTimer() {
		if (Velocity.Y >= 0) {
			_coyoteJumpTimer.Start();
		}
	}

	public void AttemptJump() {
		if (IsOnFloor() || _coyoteJumpTimer.TimeLeft > 0) {
			Jump();
		} else {
			_jumpBufferTimer.Start();
		}
	}

	private void AttemptBufferedJump() {
		if (_jumpBufferTimer.TimeLeft > 0) {
			Jump();
		}
	}

	private void Jump() {
		Vector2 velocity = Velocity;
		velocity.Y += _jumpVelocity;
		Velocity = velocity;
	}

	public void JumpCancel() {
		_jumpBufferTimer.Stop();
		
		if (!IsOnFloor() && Velocity.Y < _jumpVelocity / 2) {
			Vector2 velocity = Velocity;
			velocity.Y = _jumpVelocity / 2;
			Velocity = velocity;
		}
	}
	
	// -- Externally inputted movement methods --

	private float NewestInputAccelerationValue() {
		if (_intendedInputSpeedScale == 0 ||
		    (Mathf.Abs(_intendedInputSpeedScale) < Mathf.Abs(_currentInputSpeedScale)
		     && Mathf.Sign(_intendedInputSpeedScale) == Mathf.Sign(_currentInputSpeedScale))) {
			return IsOnFloor() ? _groundDeceleration : _airDeceleration;
		}

		return IsOnFloor() ? _groundAcceleration : _airAcceleration;
	}

	private float NewestInputAccelerationModifier() {
		float inputSpeedScaleDelta = _intendedInputSpeedScale - _currentInputSpeedScale;
		
		return Mathf.Pow(Math.Abs(inputSpeedScaleDelta), -_turnaroundAccelerationDampening);
	}

	public void AddToInputSpeedScale(float scale) {
		if (_inputSpeedScaleTween != null && _inputSpeedScaleTween.IsValid()) {
			_inputSpeedScaleTween.Kill();
		}
		_inputSpeedScaleTween = CreateTween();
		
		_intendedInputSpeedScale = MathF.Round(_intendedInputSpeedScale + scale, 4);
		_currentInputAccelerationModifier = NewestInputAccelerationModifier();
		
		_inputSpeedScaleTween.Parallel().TweenProperty(
			this,
			nameof(_currentInputSpeedScale),
			_intendedInputSpeedScale,
			1);
		UpdateInputAcceleration();
	}

	private void UpdateInputAcceleration() {
		if (_inputSpeedScaleTween != null && _inputSpeedScaleTween.IsValid()) {
			_inputSpeedScaleTween.SetSpeedScale(NewestInputAccelerationValue() * _currentInputAccelerationModifier);
		}
	}
	
	// -- Other --

	public void SetParentPositionToOwn(Node2D parent) {
		parent.Position = GlobalPosition;
		Position = Vector2.Zero;
	}
}
