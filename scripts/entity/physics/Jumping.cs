using System;
using Godot;

namespace Learning.scripts.entity.physics; 

public partial class Jumping : Node {
	[Export] private Falling Falling { get; set; }
	[Export] private LeftRight LeftRight { get; set; }

	[Export] public JumpingData Ground { get; set; }
	[Export] public JumpingData Air { get; set; }
	[Export] public JumpingData Wall { get; set; }

	[Export] private bool CoyoteGroundJumpEnabled { get; set; } = true;
	[Export] private bool GroundJumpBufferEnabled { get; set; } = true;
	[Export] private bool CoyoteWallJumpEnabled { get; set; } = true;
	[Export] private bool WallJumpBufferEnabled { get; set; } = true;
	
	private JumpingData CurrentJumpData {
		get => GetJumpData(CurrentJumpFromLocation);
		set => SetJumpData(value, CurrentJumpFromLocation);
	}

	public float JumpFacing { get; set; }
	public Location CurrentLocation { get; private set; } = Location.None;
	public Location CurrentJumpFromLocation => GetJumpFromLocation(CurrentLocation);
	public Location JumpedFrom { get; private set; } = Location.None;
	public bool WallPressing { get; private set; }

	public int NumGroundJumps { get; set; }
	public int NumAirJumps { get; set; }
	public int NumWallJumps { get; set; }
	public int CurrentNumJumps {
		get => GetNumJumps(CurrentJumpFromLocation);
		set => SetNumJumps(value, CurrentJumpFromLocation);
	}

	private Timer CoyoteJump { get; set; }
	private Timer JumpBuffer { get; set; }
	private Timer CoyoteWallJump { get; set; }
	private Timer WallJumpBuffer { get; set; }

	private Tween _jumpTweenY;
	private Tween _jumpTweenX;
	
	private float _lastWallDirection;

	[Signal]
	public delegate void JumpedEventHandler(Location from);

	[Signal]
	public delegate void CancelledJumpEventHandler(Location jumpWasFrom);

	public override void _Ready() {
		CoyoteJump = GetNode<Timer>(nameof(CoyoteJump));
		JumpBuffer = GetNode<Timer>(nameof(JumpBuffer));
		CoyoteWallJump = GetNode<Timer>(nameof(CoyoteWallJump));
		WallJumpBuffer = GetNode<Timer>(nameof(WallJumpBuffer));

		CoyoteJump.Timeout += () => CurrentLocation = Location.Air;
		CoyoteWallJump.Timeout += () => {
			WallPressing = false;
			CurrentLocation = Location.Air;
		};

		LeftRight.IntendedSpeedUpdate += WallPressCheck;
		
		ResetNumJumps();
	}
	
	public override void _PhysicsProcess(double delta) {
		CheckCoyoteWallJumpValidity();
	}

	private void CheckCoyoteWallJumpValidity() {
		bool goingPastWall = Mathf.Sign(LeftRight.CurrentSpeed) == -Mathf.Sign(_lastWallDirection);
		
		if (CoyoteWallJump.TimeLeft > 0 && goingPastWall) {
			CoyoteWallJump.Stop();
			CurrentLocation = Location.Air;
		}
	}

	private void WallPressCheck(float leftRightIntendedSpeed) {
		WallPressing = CurrentLocation == Location.WallNonGround 
					   && (Mathf.Sign(_lastWallDirection) == -Mathf.Sign(leftRightIntendedSpeed)
						   || WallPressing);
	}

	public Location GetJumpFromLocation(Location forWhich) {
		return forWhich switch {
			Location.Ground => Location.Ground,
			Location.Air => Location.Air,
			Location.WallNonGround when !WallPressing => Location.Air,
			Location.WallNonGround when WallPressing => Location.WallNonGround,
			_ => Location.None
		};
	}

	private JumpingData GetJumpData(Location forWhich) {
		return forWhich switch {
			Location.Ground => Ground,
			Location.Air => Air,
			Location.WallNonGround => Wall,
			_ => null
		};
	}

	private void SetJumpData(JumpingData newData, Location forWhich) {
		switch (forWhich) {
			case Location.Ground:
				Ground = newData;
				break;
			case Location.Air:
				Air = newData;
				break;
			case Location.WallNonGround:
				Wall = newData;
				break;
		}
	}

	public int GetNumJumps(Location forWhich) {
		return forWhich switch {
			Location.Ground => NumGroundJumps,
			Location.Air => NumAirJumps,
			Location.WallNonGround => NumWallJumps,
			_ => 0
		};
	}

	public void SetNumJumps(int numJumps, Location forWhich) {
		switch (forWhich) {
			case Location.Ground:
				NumGroundJumps = numJumps;
				break;
			case Location.Air:
				NumAirJumps = numJumps;
				break;
			case Location.WallNonGround:
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

	private void AttemptBufferedGroundJump() {
		if (CanJump() && JumpBuffer.TimeLeft > 0) {
			Jump();
		}
	}

	private void AttemptBufferedWallJump() {
		if (CanJump() && WallJumpBuffer.TimeLeft > 0) {
			Jump();
		}
	}

	private bool CanJump() {
		return CurrentNumJumps is JumpingData.UNLIMITED_JUMPS or > 0;
	}

	private void Jump() {
		PerformJump();

		if (CurrentNumJumps != JumpingData.UNLIMITED_JUMPS) {
			CurrentNumJumps--;
		}
		
		JumpedFrom = CurrentJumpFromLocation;
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
		if (CurrentJumpData.Velocity.Y != 0) {
			(_jumpTweenY, PropertyTweener t)
				= Falling.SmoothlySetBaseVelocityY(CurrentJumpData.Velocity.Y, CurrentJumpData.AccelTimeY);
			t.SetTrans(Tween.TransitionType.Expo);
		}
		if (CurrentJumpData.Velocity.X != 0) {
			(_jumpTweenX, PropertyTweener t)
				= LeftRight.SmoothlySetBaseVelocityX(CurrentJumpData.Velocity.X * JumpFacing, CurrentJumpData.AccelTimeX);
			t.SetTrans(Tween.TransitionType.Expo);
		}
	}

	public void JumpCancel() {
		JumpBuffer.Stop();
		WallJumpBuffer.Stop();

		JumpingData lastJumpData = GetJumpData(GetJumpFromLocation(JumpedFrom));
		if ((CurrentLocation == Location.Air && Falling.Velocity.Y < GetJumpCancelVelocityThreshold())
			|| InJumpTransitionY()) {
			Falling.SmoothlySetBaseVelocityY(lastJumpData.CancelVelocity, lastJumpData.CancelAccelTime);
		}

		EmitSignal(SignalName.CancelledJump, (int)JumpedFrom);
	}

	private float GetJumpCancelVelocityThreshold() {
		JumpingData lastJumpData = GetJumpData(GetJumpFromLocation(JumpedFrom));
		float extraVelocity = Falling.Gravity * Falling.GravityScale * lastJumpData.CancelAccelTime;
		return lastJumpData.CancelVelocity - extraVelocity;
	}

	public bool InJumpTransitionY() {
		return _jumpTweenY != null && _jumpTweenY.IsValid();
	}

	public bool InJumpTransitionX() {
		return _jumpTweenX != null && _jumpTweenX.IsValid();
	}

	public void TransitionToGround() {
		CoyoteJump.Stop();
		CoyoteWallJump.Stop();
		
		WallPressing = false;
		CurrentLocation = Location.Ground;
		AttemptBufferedGroundJump();
	}

	public void TransitionToAir(bool immediately = false) {
		if (!immediately
		    && CoyoteGroundJumpEnabled
		    && Falling.Velocity.Y >= 0
		    && CurrentLocation == Location.Ground) {
			CoyoteJump.Start();
		} else if (!immediately
		           && CoyoteWallJumpEnabled
		           && Falling.Velocity.Y >= 0
		           && CurrentLocation == Location.WallNonGround) {
			CoyoteWallJump.Start();
		} else {
			WallPressing = false;
			CurrentLocation = Location.Air;
		}
	}

	public void TransitionToWall(float wallDirection) {
		CoyoteJump.Stop();
		CoyoteWallJump.Stop();
		
		CurrentLocation = Location.WallNonGround;
		JumpFacing = _lastWallDirection = wallDirection;
		WallPressCheck(LeftRight.IntendedSpeed);
		AttemptBufferedWallJump();
	}
}
