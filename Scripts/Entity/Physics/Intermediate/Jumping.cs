using System;
using Godot;
using Learning.Scripts.Entity.Physics.VelocitySources;

namespace Learning.Scripts.Entity.Physics.Intermediate; 

public partial class Jumping : Node {
	[Export] internal Falling Falling { get; private set; }
	[Export] internal LeftRight LeftRight { get; private set; }

	[Export] private bool CoyoteGroundJumpEnabled { get; set; } = true;
	[Export] private bool GroundJumpBufferEnabled { get; set; } = true;
	[Export] private bool CoyoteWallJumpEnabled { get; set; } = true;
	[Export] private bool WallJumpBufferEnabled { get; set; } = true;

	[Export] public JumpingData Ground {
		get => AllJumpData[(int)Location.Ground];
		set => AllJumpData[(int)Location.Ground] = value;
	}
	[Export] public JumpingData Air {
		get => AllJumpData[(int)Location.Air];
		set => AllJumpData[(int)Location.Air] = value;
	}
	[Export] public JumpingData Wall {
		get => AllJumpData[(int)Location.WallNonGround];
		set => AllJumpData[(int)Location.WallNonGround] = value;
	}
	
	private JumpingData[] AllJumpData { get; set; } = new JumpingData[Locations.NumLocationsNotNone()];
	private JumpingData CurrentJumpData {
		get => AllJumpData[(int)CurrentLocation];
		set => AllJumpData[(int)CurrentLocation] = value;
	}

	public int NumGroundJumps {
		get => NumJumpsAll[(int)Location.Ground];
		set => NumJumpsAll[(int)Location.Ground] = value;
	}
	public int NumAirJumps {
		get => NumJumpsAll[(int)Location.Air];
		set => NumJumpsAll[(int)Location.Air] = value;
	}
	public int NumWallJumps {
		get => NumJumpsAll[(int)Location.WallNonGround];
		set => NumJumpsAll[(int)Location.WallNonGround] = value;
	}
	public int[] NumJumpsAll { get; set; } = new int[Locations.NumLocationsNotNone()];
	public int CurrentNumJumps {
		get => NumJumpsAll[(int)CurrentLocation];
		private set => NumJumpsAll[(int)CurrentLocation] = value;
	}
	
	public float JumpFacing { get; set; }
	public Location CurrentLocation { get; private set; } = Location.None;
	public Location JumpedFrom { get; private set; } = Location.None;

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
		CoyoteWallJump.Timeout += () => CurrentLocation = Location.Air;
		
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

	public void ResetNumJumps() {
		foreach (Location l in Enum.GetValues<Location>()) {
			ResetNumJumps(l);
		}
	}
	
	public void ResetNumJumps(Location which) {
		if (which == Location.None) return;

		NumJumpsAll[(int)which] = AllJumpData[(int)which].NumJumps;
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
		if (CurrentJumpData.Velocity.Y != 0) {
			(_jumpTweenY, PropertyTweener t)
				= Falling.SmoothlySetBaseVelocityY(CurrentJumpData.Velocity.Y, CurrentJumpData.AccelTimeY);
			t.SetTrans(Tween.TransitionType.Expo);
		}
		if (CurrentJumpData.Velocity.X != 0) {
			(_jumpTweenX, PropertyTweener t)
				= LeftRight.SetIntendedSpeed(CurrentJumpData.Velocity.X * JumpFacing, CurrentJumpData.AccelTimeX);
			t.SetTrans(Tween.TransitionType.Expo);
		}
	}

	public void JumpCancel() {
		JumpBuffer.Stop();
		WallJumpBuffer.Stop();

		JumpingData lastJumpData = AllJumpData[(int) JumpedFrom];
		if ((CurrentLocation == Location.Air && Falling.Velocity.Y < GetJumpCancelVelocityThreshold())
			|| InJumpTransitionY()) {
			Falling.SmoothlySetBaseVelocityY(lastJumpData.CancelVelocity, lastJumpData.CancelAccelTime);
		}

		EmitSignal(SignalName.CancelledJump, (int)JumpedFrom);
	}

	private float GetJumpCancelVelocityThreshold() {
		JumpingData lastJumpData = AllJumpData[(int) JumpedFrom];
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
			CurrentLocation = Location.Air;
		}
	}

	public void TransitionToWall(float wallDirection) {
		CoyoteJump.Stop();
		CoyoteWallJump.Stop();
		
		CurrentLocation = Location.WallNonGround;
		JumpFacing = _lastWallDirection = wallDirection;
		AttemptBufferedWallJump();
	}
}
