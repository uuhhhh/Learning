using Godot;
using System;

namespace Learning.scripts.entity;

public partial class KinematicComp : CharacterBody2D {
	[Export] private KinematicCompData _physData;
	
	private float _gravity = ProjectSettings.GetSetting("physics/2d/default_gravity").AsSingle();
	
	private Timer _coyoteJumpTimer;
	private Timer _jumpBufferTimer;

	private Tween _jumpTween;
	private bool _jumpTweenReady;
	private float _currentAirVelocityY;
	private int _currentAirJumps;

	private bool _draggingOnWall;

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
	[Signal]
	public delegate void StartDragOnWallEventHandler();
	[Signal]
	public delegate void StopDragOnWallEventHandler();

	public override void _Ready() {
		_coyoteJumpTimer = GetNode<Timer>("CoyoteJumpTimer");
		_jumpBufferTimer = GetNode<Timer>("JumpBufferTimer");

		BecomeOnFloor += AttemptBufferedJump;
		BecomeOffFloor += HandleCoyoteJumpTimer;
		BecomeOnFloor += () => { _coyoteJumpTimer.Stop(); };

		BecomeOnCeiling += () => { _currentAirVelocityY = 0; _jumpTween?.Kill(); };
		BecomeOnFloor += () => { _currentAirVelocityY = 0; };

		BecomeOnFloor += UpdateInputAcceleration;
		BecomeOffFloor += UpdateInputAcceleration;

		BecomeOnFloor += ResetCurrentAirJumps;
		BecomeOnWall += ResetCurrentAirJumps;
		ResetCurrentAirJumps();

		StartDragOnWall += () => { _draggingOnWall = true; };
		StopDragOnWall += () => { _draggingOnWall = false; };
	}
	
	#region Per-Frame Physics Handling

	public override void _PhysicsProcess(double delta) {
		StartReadyTweens();
		
		Velocity = InputVelocity + new Vector2(0, _currentAirVelocityY);
		HandleGravity(delta);

		bool wasOnFloor = IsOnFloor(), wasOnCeiling = IsOnCeiling(), wasOnWall = IsOnWall();
		MoveAndSlide();
		CheckFloorStatusChange(wasOnFloor);
		CheckCeilingStatusChange(wasOnCeiling);
		CheckWallStatusChange(wasOnWall);
		CheckDragOnWallStatusChange();
	}

	private void StartReadyTweens() {
		if (_jumpTweenReady) {
			_jumpTween.Play();
			_jumpTweenReady = false;
		}
	}

	private void HandleGravity(double delta) {
		if (IsOnFloor()) return;
		
		_currentAirVelocityY += _gravity * GetGravityScale() * (float)delta;

		_currentAirVelocityY = Mathf.Min(_currentAirVelocityY, 
			_draggingOnWall ? _physData.Wall.MaxDragSpeed : _physData.MaxFallVelocity);
	}

	private float GetGravityScale() {
		bool descending = _currentAirVelocityY > 0;
		float gravityScale = descending ? _physData.DownwardsGravityScale : _physData.UpwardsGravityScale;

		if (_draggingOnWall) {
			gravityScale *= descending ? _physData.Wall.DragDescendGravityScale
				: _physData.Wall.DragAscendGravityScale;
		}

		return gravityScale;
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

	private void CheckDragOnWallStatusChange() {
		bool draggingNoInput = _draggingOnWall && _intendedInputSpeedScale == 0; 
		
		bool validDrag = _physData.Wall.CanDrag
			&& IsOnWallOnly()
            && (Mathf.Sign(_intendedInputSpeedScale) == -Mathf.Sign(GetWallNormal().X) || draggingNoInput)
            && GetWallNormal().Y == 0
			&& _currentAirVelocityY >= _physData.Wall.DragVelocityThresholdMin;
		
		if (_draggingOnWall && !validDrag) {
			EmitSignal(SignalName.StopDragOnWall);
		} else if (!_draggingOnWall && validDrag) {
			EmitSignal(SignalName.StartDragOnWall);
		}
	}
	
	#endregion

	#region Jumping Methods

	private void HandleCoyoteJumpTimer() {
		if (_currentAirVelocityY >= 0) {
			_coyoteJumpTimer.Start();
		}
	}

	public void AttemptJump() {
		bool canCoyoteJump = _coyoteJumpTimer.TimeLeft > 0;
		if (IsOnFloor()) {
			GroundJump();
		} else if (canCoyoteJump) {
			CoyoteJump();
		} else if (_draggingOnWall && _physData.Wall.CanJump) {
			WallJump(GetWallNormal().X);	
		} else if (CanAirJump()) {
			AirJump();
		} else {
			_jumpBufferTimer.Start();
		}
	}

	private void AttemptBufferedJump() {
		if (_jumpBufferTimer.TimeLeft > 0) {
			GroundJump();
		}
	}

	private void GroundJump() {
		SetAirVelocity(_physData.JumpVelocity, _physData.JumpAcceleration);
	}

	private void CoyoteJump() {
		SetAirVelocity(_physData.JumpVelocity, _physData.CoyoteJumpAcceleration);
	}

	private void WallJump(float velocityScaleX) {
		SetAirVelocity(_physData.Wall.JumpVelocity.Y, _physData.Wall.JumpAcceleration);
		
		_currentInputSpeedScale = velocityScaleX;
		
		_inputSpeedScaleTween?.Kill();
		_inputSpeedScaleTween = CreateTween();
		TweenSpeedScaleToIntended();
	}

	private bool CanAirJump() {
		return _currentAirJumps is KinematicCompData.UnlimitedAirJumps or > 0
		       && _currentAirVelocityY > _physData.AirJumpVelocity;
	}

	private void AirJump() {
		SetAirVelocity(_physData.AirJumpVelocity, _physData.AirJumpAcceleration);
		_currentAirJumps--;
	}

	private void SetAirVelocity(float velocity, float acceleration) {
		_jumpTween?.Kill();
		_jumpTween = CreateTween();
		_jumpTween.Pause();

		_jumpTween.TweenProperty(this,
			nameof(_currentAirVelocityY),
			velocity,
			1)
			.FromCurrent()
			.SetTrans(Tween.TransitionType.Expo);
		_jumpTween.SetSpeedScale(acceleration);
		_jumpTweenReady = true;
	}

	public void JumpCancel() {
		_jumpBufferTimer.Stop();
		
		if ((!IsOnFloor() && _currentAirVelocityY < GetJumpCancelVelocityThreshold())
		    || (_jumpTween != null && _jumpTween.IsValid())) {
			SetAirVelocity(_physData.JumpCancelVelocity, _physData.JumpCancelAcceleration);
		}
	}

	private float GetJumpCancelVelocityThreshold() {
		float baseThreshold = _physData.JumpCancelVelocity;
		float timeToCancellationVelocity = 1 / _physData.JumpCancelAcceleration;
		float extraVelocity = _gravity * GetGravityScale() * timeToCancellationVelocity;

		return baseThreshold - extraVelocity;
	}

	private void ResetCurrentAirJumps() {
		_currentAirJumps = _physData.NumAirJumps;
	}
	
	#endregion
	
	#region Externally Inputted Movement Methods

	public void AddToInputSpeedScale(float scale) {
		_inputSpeedScaleTween?.Kill();
		_inputSpeedScaleTween = CreateTween();
		
		_intendedInputSpeedScale = MathF.Round(_intendedInputSpeedScale + scale, 4);
		_currentInputAccelerationModifier = NewestInputAccelerationModifier();
		
		TweenSpeedScaleToIntended();
	}

	private void TweenSpeedScaleToIntended() {
		_inputSpeedScaleTween.TweenProperty(
			this,
			nameof(_currentInputSpeedScale),
			_intendedInputSpeedScale,
			1).FromCurrent();
		UpdateInputAcceleration();
	}

	private void UpdateInputAcceleration() {
		if (_inputSpeedScaleTween != null && _inputSpeedScaleTween.IsValid()) {
			_inputSpeedScaleTween.SetSpeedScale(NewestInputAccelerationValue() * _currentInputAccelerationModifier);
		}
	}

	private float NewestInputAccelerationModifier() {
		float inputSpeedScaleDelta = _intendedInputSpeedScale - _currentInputSpeedScale;
		
		return Mathf.Pow(Math.Abs(inputSpeedScaleDelta), -_physData.TurnaroundAccelerationDampening);
	}

	private float NewestInputAccelerationValue() {
		if (_intendedInputSpeedScale == 0 ||
		    (Mathf.Abs(_intendedInputSpeedScale) < Mathf.Abs(_currentInputSpeedScale)
		     && Mathf.Sign(_intendedInputSpeedScale) == Mathf.Sign(_currentInputSpeedScale))) {
			return IsOnFloor() ? _physData.GroundDeceleration : _physData.AirDeceleration;
		}

		return IsOnFloor() ? _physData.GroundAcceleration : _physData.AirAcceleration;
	}
	
	#endregion

	public void SetParentPositionToOwn(Node2D parent) {
		parent.Position = GlobalPosition;
		Position = Vector2.Zero;
	}
}
