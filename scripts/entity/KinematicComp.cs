using Godot;
using System;

namespace Learning.scripts.entity;

public partial class KinematicComp : CharacterBody2D {
	[Export] private KinematicCompData _physData;
	
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
		
		newVelocity.X = _currentInputSpeedScale * _physData.Speed;

		return newVelocity;
	}

	private Vector2 HandleGravity(Vector2 velocity, double delta) {
		Vector2 newVelocity = velocity;
		
		if (!IsOnFloor()) {
			float gravityScale = newVelocity.Y > 0 ? _physData.DownwardsGravityScale : _physData.UpwardsGravityScale;
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
		velocity.Y += _physData.JumpVelocity;
		Velocity = velocity;
	}

	public void JumpCancel() {
		_jumpBufferTimer.Stop();
		
		if (!IsOnFloor() && Velocity.Y < _physData.JumpVelocity * _physData.JumpCancelVelocityProportion) {
			Vector2 velocity = Velocity;
			velocity.Y = _physData.JumpVelocity * _physData.JumpCancelVelocityProportion;
			Velocity = velocity;
		}
	}
	
	// -- Externally inputted movement methods --

	private float NewestInputAccelerationValue() {
		if (_intendedInputSpeedScale == 0 ||
		    (Mathf.Abs(_intendedInputSpeedScale) < Mathf.Abs(_currentInputSpeedScale)
		     && Mathf.Sign(_intendedInputSpeedScale) == Mathf.Sign(_currentInputSpeedScale))) {
			return IsOnFloor() ? _physData.GroundDeceleration : _physData.AirDeceleration;
		}

		return IsOnFloor() ? _physData.GroundAcceleration : _physData.AirAcceleration;
	}

	private float NewestInputAccelerationModifier() {
		float inputSpeedScaleDelta = _intendedInputSpeedScale - _currentInputSpeedScale;
		
		return Mathf.Pow(Math.Abs(inputSpeedScaleDelta), -_physData.TurnaroundAccelerationDampening);
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
