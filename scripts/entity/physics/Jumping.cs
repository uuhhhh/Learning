using System;
using Godot;

namespace Learning.scripts.entity.physics; 

public partial class Jumping : Node2D, IKinematicCompLinkable {
	[Export] public bool DoNotLink { get; set; }
	[Export] private Falling AirMovement { get; set; }
	[Export] private LeftRight HorizontalMovement { get; set; }

	[Export] private JumpingData Ground { get; set; }
	[Export] private JumpingData Air { get; set; }
	[Export] private JumpingData Wall { get; set; }

	[Export] private bool CoyoteGroundJumpEnabled { get; set; } = true;
	[Export] private bool GroundJumpBufferEnabled { get; set; } = true;
	[Export] private bool CoyoteWallJumpEnabled { get; set; } = true;
	[Export] private bool WallJumpBufferEnabled { get; set; } = true;
	
	private JumpingData JumpData {
		get => GetJumpData(CurrentLocation);
		set => SetJumpData(value, CurrentLocation);
	}

	public float JumpFacing { get; set; }
	public Location CurrentLocation { get; set; } = Location.None;
	public Location JumpedFrom { get; private set; } = Location.None;
	public bool WallPressing { get; set; } = true;

	public int NumGroundJumps { get; set; }
	public int NumAirJumps { get; set; }
	public int NumWallJumps { get; set; }
	public int NumJumps {
		get => GetNumJumps(CurrentLocation);
		set => SetNumJumps(value, CurrentLocation);
	}

	private Timer CoyoteJump { get; set; }
	private Timer JumpBuffer { get; set; }
	private Timer CoyoteWallJump { get; set; }
	private Timer WallJumpBuffer { get; set; }

	private Tween _jumpTweenY;
	private Tween _jumpTweenX;
	
	private float _lastWallNormal;
	private float _wallPositionX;

	[Signal]
	public delegate void JumpedEventHandler(Location from);

	[Signal]
	public delegate void CancelledJumpEventHandler(Location jumpWasFrom);

	public override void _Ready() {
		CoyoteJump = GetNode<Timer>(nameof(CoyoteJump));
		JumpBuffer = GetNode<Timer>(nameof(JumpBuffer));
		CoyoteWallJump = GetNode<Timer>(nameof(CoyoteWallJump));
		WallJumpBuffer = GetNode<Timer>(nameof(WallJumpBuffer));

		CoyoteJump.Timeout += () => {
			CurrentLocation = Location.Air;
		};
		CoyoteWallJump.Timeout += () => {
			CurrentLocation = Location.Air;
		};

		HorizontalMovement.IntendedSpeedUpdate += WallPressCheck;
		
		ResetNumJumps();
	}
	
	public override void _PhysicsProcess(double delta) {
		CheckCoyoteWallJumpValidity();
	}

	private void WallPressCheck(float leftRightIntendedSpeed) {
		WallPressing = CurrentLocation == Location.WallNonGround 
					   && (Mathf.Sign(_lastWallNormal) == -Mathf.Sign(leftRightIntendedSpeed)
						   || WallPressing);
	}

	private JumpingData GetJumpData(Location forWhich) {
		return forWhich switch {
			Location.Ground => Ground,
			Location.Air => Air,
			Location.WallNonGround when !WallPressing => Air,
			Location.WallNonGround when WallPressing => Wall,
			_ => null
		};
	}

	private void SetJumpData(JumpingData newData, Location forWhich) {
		switch (forWhich) {
			case Location.Ground:
				Ground = newData;
				break;
			case Location.Air:
			case Location.WallNonGround when !WallPressing:
				Air = newData;
				break;
			case Location.WallNonGround when WallPressing:
				Wall = newData;
				break;
		}
	}

	public int GetNumJumps(Location forWhich) {
		return forWhich switch {
			Location.Ground => NumGroundJumps,
			Location.Air => NumAirJumps,
			Location.WallNonGround when !WallPressing => NumAirJumps,
			Location.WallNonGround when WallPressing => NumWallJumps,
			_ => 0
		};
	}

	public void SetNumJumps(int numJumps, Location forWhich) {
		switch (forWhich) {
			case Location.Ground:
				NumGroundJumps = numJumps;
				break;
			case Location.Air:
			case Location.WallNonGround when !WallPressing:
				NumAirJumps = numJumps;
				break;
			case Location.WallNonGround when WallPressing:
				NumWallJumps = numJumps;
				break;
		}
	}

	public void ResetNumJumps() {
		foreach (Location l in Enum.GetValues<Location>()) {
			ResetNumJumps(l);
		}
	}
	
	public void ResetNumJumps(Location which) {
		if (which == Location.None) return;
		
		SetNumJumps(GetJumpData(which).NumJumps, which);
	}

	public void AttemptJump() {
		if (CanJump()) {
			Jump();
		} else {
			if (GroundJumpBufferEnabled) {
				JumpBuffer.Start();
			}
			if (WallJumpBufferEnabled) {
				WallJumpBuffer.Start();
			}
		}
	}

	private bool CanJump() {
		return NumJumps is JumpingData.UNLIMITED_JUMPS or > 0;
	}

	private void Jump() {
		PerformJump();

		if (NumJumps != JumpingData.UNLIMITED_JUMPS) {
			NumJumps--;
		}
		
		JumpedFrom = CurrentLocation;
		EmitSignal(SignalName.Jumped, (int)JumpedFrom);
		if (CoyoteJump.TimeLeft > 0 || CoyoteWallJump.TimeLeft > 0) {
			CurrentLocation = Location.Air;
		}
		
		JumpBuffer.Stop();
		WallJumpBuffer.Stop();
		CoyoteJump.Stop();
		CoyoteWallJump.Stop();
	}

	private void PerformJump() {
		if (JumpData.Velocity.Y != 0) {
			(_jumpTweenY, PropertyTweener t)
				= AirMovement.SmoothlySetBaseVelocityY(JumpData.Velocity.Y, JumpData.AccelTimeY);
			t.SetTrans(Tween.TransitionType.Expo);
		}
		if (JumpData.Velocity.X != 0) {
			(_jumpTweenX, PropertyTweener t)
				= HorizontalMovement.SmoothlySetBaseVelocityX(JumpData.Velocity.X * JumpFacing, JumpData.AccelTimeX);
			t.SetTrans(Tween.TransitionType.Expo);
		}
	}

	public void JumpCancel() {
		JumpBuffer.Stop();
		WallJumpBuffer.Stop();

		JumpingData lastJumpData = GetJumpData(JumpedFrom);
		if ((CurrentLocation == Location.Air && AirMovement.Velocity.Y < GetJumpCancelVelocityThreshold())
			|| InJumpTransitionY()) {
			AirMovement.SmoothlySetBaseVelocityY(lastJumpData.CancelVelocity, lastJumpData.CancelAccelTime);
		}

		EmitSignal(SignalName.CancelledJump, (int)JumpedFrom);
	}

	public bool InJumpTransitionY() {
		return _jumpTweenY != null && _jumpTweenY.IsValid();
	}

	public bool InJumpTransitionX() {
		return _jumpTweenX != null && _jumpTweenX.IsValid();
	}

	private float GetJumpCancelVelocityThreshold() {
		JumpingData lastJumpData = GetJumpData(JumpedFrom);
		float extraVelocity = AirMovement.Gravity * AirMovement.GravityScale * lastJumpData.CancelAccelTime;
		return lastJumpData.CancelVelocity - extraVelocity;
	}

	public void DefaultOnBecomeOnFloor(KinematicComp physics) {
		CoyoteJump.Stop();
		CoyoteWallJump.Stop();
			
		CurrentLocation = Location.Ground;
		if (JumpBuffer.TimeLeft > 0) {
			Jump();
		}
	}

	public void DefaultOnBecomeOffFloor(KinematicComp physics) {
		if (physics.IsOnWall()) {
			TransitionToWall(physics);
		} else if (CoyoteGroundJumpEnabled && AirMovement.Velocity.Y >= 0 && CurrentLocation != Location.None) {
			CoyoteJump.Start();
		} else {
			CurrentLocation = Location.Air;
		}
	}

	public void DefaultOnBecomeOnWall(KinematicComp physics) {
		CoyoteJump.Stop();
		CoyoteWallJump.Stop();
			
		if (!physics.IsOnFloor()) {
			TransitionToWall(physics);
			if (WallJumpBuffer.TimeLeft > 0) {
				Jump();
			}
		}
	}

	public void DefaultOnBecomeOffWall(KinematicComp physics) {
		if (!physics.IsOnFloor()) {
			if (CoyoteWallJumpEnabled && AirMovement.Velocity.Y >= 0 && CurrentLocation != Location.None) {
				CoyoteWallJump.Start();
			} else {
				CurrentLocation = Location.Air;
			}
		}
	}

	private void CheckCoyoteWallJumpValidity() {
		float epsilon = 0.001f;
		bool isPastWall = Mathf.Sign(GlobalPosition.X - _wallPositionX) == -Mathf.Sign(_lastWallNormal);
		bool isFarEnoughFromWallPos = Mathf.Abs(GlobalPosition.X - _wallPositionX) > epsilon;
		
		if (CoyoteWallJump.TimeLeft > 0 && isPastWall && isFarEnoughFromWallPos) {
			CoyoteWallJump.Stop();
			CurrentLocation = Location.Air;
		}
	}

	private void TransitionToWall(KinematicComp physics) {
		CurrentLocation = Location.WallNonGround;
		_lastWallNormal = physics.GetWallNormal().X;
		JumpFacing = _lastWallNormal;
		WallPressCheck(HorizontalMovement.IntendedSpeed);
		_wallPositionX = physics.GlobalPosition.X;
	}
}
