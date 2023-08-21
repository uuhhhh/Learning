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
	
	private JumpingData JumpData {
		get => GetJumpData(CurrentLocation);
		set => SetJumpData(value, CurrentLocation);
	}

	public float JumpFacing { get; set; }
	public Location CurrentLocation { get; private set; } = Location.None;
	public Location JumpedFrom { get; private set; } = Location.None;
	public bool WallPressing { get; private set; }

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

		CoyoteJump.Timeout += () => {
			CurrentLocation = Location.Air;
		};
		CoyoteWallJump.Timeout += () => {
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
				= Falling.SmoothlySetBaseVelocityY(JumpData.Velocity.Y, JumpData.AccelTimeY);
			t.SetTrans(Tween.TransitionType.Expo);
		}
		if (JumpData.Velocity.X != 0) {
			(_jumpTweenX, PropertyTweener t)
				= LeftRight.SmoothlySetBaseVelocityX(JumpData.Velocity.X * JumpFacing, JumpData.AccelTimeX);
			t.SetTrans(Tween.TransitionType.Expo);
		}
	}

	public void JumpCancel() {
		JumpBuffer.Stop();
		WallJumpBuffer.Stop();

		JumpingData lastJumpData = GetJumpData(JumpedFrom);
		if ((CurrentLocation == Location.Air && Falling.Velocity.Y < GetJumpCancelVelocityThreshold())
			|| InJumpTransitionY()) {
			Falling.SmoothlySetBaseVelocityY(lastJumpData.CancelVelocity, lastJumpData.CancelAccelTime);
		}

		EmitSignal(SignalName.CancelledJump, (int)JumpedFrom);
	}

	private float GetJumpCancelVelocityThreshold() {
		JumpingData lastJumpData = GetJumpData(JumpedFrom);
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
		
		CurrentLocation = Location.Ground;
		AttemptBufferedGroundJump();
	}

	public void TransitionToAir() {
		if (CoyoteGroundJumpEnabled && Falling.Velocity.Y >= 0 && CurrentLocation == Location.Ground) {
			CoyoteJump.Start();
		} else if (CoyoteWallJumpEnabled && Falling.Velocity.Y >= 0 && CurrentLocation == Location.WallNonGround) {
			CoyoteWallJump.Start();
		} else {
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
