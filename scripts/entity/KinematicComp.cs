using Godot;
using System;

namespace Learning.scripts.entity;

public partial class KinematicComp : CharacterBody2D {
	[Export] private KinematicCompData _physData;
	
	private float _gravity = ProjectSettings.GetSetting("physics/2d/default_gravity").AsSingle();
	
	private Timer _coyoteJumpTimer;
	private Timer _jumpBufferTimer;
	private Timer _coyoteWallJumpTimer;
	private Timer _wallJumpBufferTimer;

	private Vector2 AirVelocity => new (0, _currentAirVelocityY);
	private float _currentAirVelocityY;
	private Tween _jumpTween;
	private bool _jumpTweenReady;
	private int _currentAirJumps;

	private bool _draggingOnWall;
	private Vector2 _lastWallNormal;

	private Vector2 InputVelocity => new (_currentInputSpeedScale * _physData.Speed, 0);
	private float _currentInputSpeedScale;
	private float _intendedInputSpeedScale;
	private float _currentInputAccelerationModifier;
	private Tween _inputSpeedScaleTween;
	private Tween _inputSpeedScaleTweenOverride;
	private bool _tweenBeingOverridden;
	
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
		_coyoteWallJumpTimer = GetNode<Timer>("CoyoteWallJumpTimer");
		_wallJumpBufferTimer = GetNode<Timer>("WallJumpBufferTimer");

		BecomeOnFloor += AttemptBufferedJump;
		StartDragOnWall += AttemptBufferedWallJump;
		
		BecomeOffFloor += HandleCoyoteJumpTimer;
		BecomeOnFloor += () => { _coyoteJumpTimer.Stop(); };

		StopDragOnWall += () => { _coyoteWallJumpTimer.Start(); };
		StartDragOnWall += () => { _coyoteWallJumpTimer.Stop(); };

		BecomeOnCeiling += () => { _currentAirVelocityY = 0; _jumpTween?.Kill(); };
		BecomeOnFloor += () => { _currentAirVelocityY = 0; };

		BecomeOnFloor += UpdateInputAcceleration;
		BecomeOffFloor += UpdateInputAcceleration;

		BecomeOnFloor += ResetCurrentAirJumps;
		StartDragOnWall += ResetCurrentAirJumps;
		ResetCurrentAirJumps();

		StartDragOnWall += () => { _draggingOnWall = true; };
		StopDragOnWall += () => { _draggingOnWall = false; };
	}
	
	#region Per-Frame Physics Handling

	public override void _PhysicsProcess(double delta) {
		StartReadyTweens();
		SetLastWallNormal();
		
		Velocity = InputVelocity + AirVelocity;
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

	private void SetLastWallNormal() {
		if (IsOnWall()) {
			_lastWallNormal = GetWallNormal();
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
		switch (wasOnFloor, IsOnFloor()) {
			case (true, false):
				EmitSignal(SignalName.BecomeOffFloor);
				break;
			case (false, true):
				EmitSignal(SignalName.BecomeOnFloor);
				break;
		}
	}

	private void CheckCeilingStatusChange(bool wasOnCeiling) {
		switch (wasOnCeiling, IsOnCeiling()) {
			case (true, false):
				EmitSignal(SignalName.BecomeOffCeiling);
				break;
			case (false, true):
				EmitSignal(SignalName.BecomeOnCeiling);
				break;
		}
	}

	private void CheckWallStatusChange(bool wasOnWall) {
		switch (wasOnWall, IsOnWall()) {
			case (true, false):
				EmitSignal(SignalName.BecomeOffWall);
				break;
			case (false, true):
				EmitSignal(SignalName.BecomeOnWall);
				break;
		}
	}

	private void CheckDragOnWallStatusChange() {
		bool draggingNoInput = _draggingOnWall && _intendedInputSpeedScale == 0; 
		
		bool validDrag = _physData.Wall.CanDrag
			&& IsOnWallOnly()
            && (Mathf.Sign(_intendedInputSpeedScale) == -Mathf.Sign(GetWallNormal().X) || draggingNoInput)
            && GetWallNormal().Y == 0
			&& _currentAirVelocityY >= _physData.Wall.DragVelocityThresholdMin;
		
		switch (_draggingOnWall, validDrag) {
			case (true, false):
				EmitSignal(SignalName.StopDragOnWall);
				break;
			case (false, true):
				EmitSignal(SignalName.StartDragOnWall);
				break;
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
		bool canCoyoteWallJump = _coyoteWallJumpTimer.TimeLeft > 0;
        
		if (IsOnFloor()) {
			GroundJump();
		} else if (canCoyoteJump) {
			CoyoteJump();
		} else if (_draggingOnWall && _physData.Wall.CanJump) {
			WallJump(GetWallNormal().X);	
		} else if (canCoyoteWallJump && _physData.Wall.CanJump) {
			CoyoteWallJump(_lastWallNormal.X);	
		} else if (CanAirJump()) {
			AirJump();
		} else {
			_jumpBufferTimer.Start();
			_wallJumpBufferTimer.Start();
		}
	}

	private void AttemptBufferedJump() {
		if (_jumpBufferTimer.TimeLeft > 0) {
			GroundJump();
		}
	}

	private void AttemptBufferedWallJump() {
		if (_wallJumpBufferTimer.TimeLeft > 0) {
			WallJump(GetWallNormal().X);
		}
	}

	private void GroundJump() {
		SetAirVelocity(_physData.JumpVelocity, _physData.JumpAcceleration);
	}

	private void CoyoteJump() {
		SetAirVelocity(_physData.JumpVelocity, _physData.CoyoteJumpAcceleration);
	}

	private void WallJump(float velocityScaleX) {
		WallJump(velocityScaleX, _physData.Wall.JumpAcceleration);
	}

	private void WallJump(float velocityScaleX, Vector2 acceleration) {
		SetAirVelocity(_physData.Wall.JumpVelocity.Y, acceleration.Y);
		
		TweenSpeedScaleTempOverride(velocityScaleX, acceleration.X);
	}

	private void CoyoteWallJump(float velocityScaleX) {
		WallJump(velocityScaleX, _physData.Wall.CoyoteJumpAcceleration);
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
		_jumpTween.SetProcessMode(Tween.TweenProcessMode.Physics);
		_jumpTweenReady = true;
	}

	public void JumpCancel() {
		_jumpBufferTimer.Stop();
		_wallJumpBufferTimer.Stop();
		
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
		_intendedInputSpeedScale = MathF.Round(_intendedInputSpeedScale + scale, 4);
		
		if (_tweenBeingOverridden) return;
		
		_inputSpeedScaleTween?.Kill();
		_inputSpeedScaleTween = CreateTween();
		
		_currentInputAccelerationModifier = NewestInputAccelerationModifier();
		TweenSpeedScaleToIntended();
	}

	private void TweenSpeedScaleToIntended() {
		_inputSpeedScaleTween.TweenProperty(
			this,
			nameof(_currentInputSpeedScale),
			_intendedInputSpeedScale,
			1).FromCurrent();
		_inputSpeedScaleTween.SetProcessMode(Tween.TweenProcessMode.Physics);
		UpdateInputAcceleration();
	}

	private void TweenSpeedScaleTempOverride(float speedScale, float acceleration) {
		_tweenBeingOverridden = true;
		_inputSpeedScaleTween?.Kill();
		_inputSpeedScaleTweenOverride = CreateTween();

		_inputSpeedScaleTweenOverride.TweenProperty(
			this,
			nameof(_currentInputSpeedScale),
			speedScale,
			1).FromCurrent();
		_inputSpeedScaleTweenOverride.SetProcessMode(Tween.TweenProcessMode.Physics);
		_inputSpeedScaleTweenOverride.SetSpeedScale(acceleration);
		_inputSpeedScaleTweenOverride.TweenCallback(Callable.From(() => {
			_inputSpeedScaleTween = CreateTween();
			TweenSpeedScaleToIntended();
			_tweenBeingOverridden = false;
		}));
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
