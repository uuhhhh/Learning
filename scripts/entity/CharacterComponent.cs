using Godot;
using System;

public partial class CharacterComponent : CharacterBody2D {
	[Export] private float _speed;
	[Export] private float _jumpVelocity;
	[Export] private float _timeToFullSpeedSeconds;
	[Export] private float _timeToStopSeconds;
	
	private float _gravity = ProjectSettings.GetSetting("physics/2d/default_gravity").AsSingle();
	
	private Timer _coyoteJumpTimer;
	private Timer _jumpBufferTimer;

	private float _inputVelocityX;
	private float _finalInputVelocityX;
	private Tween _inputVelocityTween;

	public override void _Ready() {
		_coyoteJumpTimer = GetNode<Timer>("CoyoteJumpTimer");
		_jumpBufferTimer = GetNode<Timer>("JumpBufferTimer");
	}

	public override void _PhysicsProcess(double delta) {
		Velocity = HandleInputVelocity(Velocity, delta);
		Velocity = HandleGravity(Velocity, delta);

		bool wasOnFloor = IsOnFloor();
		MoveAndSlide();
		HandleBufferedJump(wasOnFloor);
		HandleCoyoteJumpTimer(wasOnFloor);
	}

	private Vector2 HandleInputVelocity(Vector2 velocity, double delta) {
		Vector2 newVelocity = velocity;
		
		newVelocity.X = _inputVelocityX;

		return newVelocity;
	}

	private Vector2 HandleGravity(Vector2 velocity, double delta) {
		Vector2 newVelocity = velocity;
		
		if (!IsOnFloor()) {
			newVelocity.Y += _gravity * (float)delta;
		}

		return newVelocity;
	}

	private void HandleBufferedJump(bool wasOnFloor) {
		if (!wasOnFloor && IsOnFloor() && _jumpBufferTimer.TimeLeft > 0) {
			Jump();
		}
	}

	private void HandleCoyoteJumpTimer(bool wasOnFloor) {
		if (wasOnFloor && !IsOnFloor() && Velocity.Y >= 0) {
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

	public void MoveX(float magnitude) {
		if (_inputVelocityTween != null && _inputVelocityTween.IsValid()) {
			_inputVelocityTween.Kill();
		}

		_finalInputVelocityX = Mathf.Round(_finalInputVelocityX + _speed * magnitude);

		_inputVelocityTween = CreateTween();
		_inputVelocityTween.Parallel().TweenProperty(
			this,
			"_inputVelocityX",
			_finalInputVelocityX,
			_finalInputVelocityX == 0 ? _timeToStopSeconds : _timeToFullSpeedSeconds);
	}

	public void SetParentPositionToOwn(Node2D parent) {
		parent.Position = GlobalPosition;
		Position = Vector2.Zero;
	}
}
