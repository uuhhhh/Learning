using Godot;
using System;

namespace Learning.scripts.entity;

public partial class KinematicComp : CharacterBody2D {
	[Export] private KinematicCompData _physData;
	
	private float _gravity = ProjectSettings.GetSetting("physics/2d/default_gravity").AsSingle();
	
	private Timer _coyoteJumpTimer;
	private Timer _jumpBufferTimer;

	private Vector2 _currentAirVelocity;
	private int _currentAirJumps;

	private Vector2 InputVelocity => new (_currentInputSpeedScale * _physData.Speed, 0);
	private float _currentInputSpeedScale;
	private float _intendedInputSpeedScale;
	private float _currentInputAccelerationModifier;
	private Tween _inputSpeedScaleTween;
	
	[Signal]
	public delegate void BecomeOnFloorEventHandler();
	[Signal]
	public delegate void BecomeOffFloorEventHandler();
	[Signal]
	public delegate void BecomeOnCeilingEventHandler();
	[Signal]
	public delegate void BecomeOffCeilingEventHandler();
	[Signal]
	public delegate void BecomeOnWallEventHandler();
	[Signal]
	public delegate void BecomeOffWallEventHandler();

	public override void _Ready() {
		_coyoteJumpTimer = GetNode<Timer>("CoyoteJumpTimer");
		_jumpBufferTimer = GetNode<Timer>("JumpBufferTimer");

		BecomeOnFloor += AttemptBufferedJump;
		BecomeOffFloor += HandleCoyoteJumpTimer;
		BecomeOnFloor += () => { _coyoteJumpTimer.Stop(); };

		BecomeOnCeiling += () => { _currentAirVelocity.Y = 0; };
		BecomeOnFloor += () => { _currentAirVelocity.Y = 0; };

		BecomeOnFloor += UpdateInputAcceleration;
		BecomeOffFloor += UpdateInputAcceleration;

		BecomeOnFloor += ResetCurrentAirJumps;
		BecomeOnWall += ResetCurrentAirJumps;
		ResetCurrentAirJumps();
	}

	public override void _PhysicsProcess(double delta) {
		Velocity = InputVelocity + _currentAirVelocity;
		_currentAirVelocity = HandleGravity(_currentAirVelocity, delta);

		bool wasOnFloor = IsOnFloor(), wasOnCeiling = IsOnCeiling(), wasOnWall = IsOnWall();
		MoveAndSlide();
		CheckFloorStatusChange(wasOnFloor);
		CheckCeilingStatusChange(wasOnCeiling);
		CheckWallStatusChange(wasOnWall);
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

	private void CheckCeilingStatusChange(bool wasOnCeiling) {
		if (wasOnCeiling && !IsOnCeiling()) {
			EmitSignal(SignalName.BecomeOffCeiling);
		} else if (!wasOnCeiling && IsOnCeiling()) {
			EmitSignal(SignalName.BecomeOnCeiling);
		}
	}

	private void CheckWallStatusChange(bool wasOnWall) {
		if (wasOnWall && !IsOnWall()) {
			EmitSignal(SignalName.BecomeOffWall);
		} else if (!wasOnWall && IsOnWall()) {
			EmitSignal(SignalName.BecomeOnWall);
		}
	}
	
	// -- Jumping methods --

	private void HandleCoyoteJumpTimer() {
		if (_currentAirVelocity.Y >= 0) {
			_coyoteJumpTimer.Start();
		}
	}

	public void AttemptJump() {
		bool canCoyoteJump = _coyoteJumpTimer.TimeLeft > 0;
		if (IsOnFloor() || canCoyoteJump) {
			Jump();
		} else if (IsOnWall() && _physData.CanWallJump) {
			WallJump(GetWallNormal().X);	
		} if (CanAirJump()) {
			AirJump();
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
		_currentAirVelocity.Y = _physData.JumpVelocity;
	}

	private void WallJump(float velocityScaleX) {
		_currentAirVelocity.Y = _physData.WallJumpVelocity.Y;
		//_currentJumpVelocity.X = _physData.WallJumpVelocity.X * velocityScaleX;
	}

	private void AirJump() {
		_currentAirVelocity.Y =_physData.AirJumpVelocity;
		_currentAirJumps--;
	}

	public void JumpCancel() {
		_jumpBufferTimer.Stop();
		
		if (!IsOnFloor() && _currentAirVelocity.Y < _physData.JumpVelocity * _physData.JumpCancelVelocityProportion) {
			_currentAirVelocity.Y = _physData.JumpVelocity * _physData.JumpCancelVelocityProportion;
		}
	}

	private bool CanAirJump() {
		return _currentAirJumps is KinematicCompData.UnlimitedAirJumps or > 0
		       && _currentAirVelocity.Y > _physData.AirJumpVelocity;
	}

	private void ResetCurrentAirJumps() {
		_currentAirJumps = _physData.NumAirJumps;
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
