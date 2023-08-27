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
		get => GetJumpingDataFor(Location.Ground);
		set => SetJumpingDataFor(value, Location.Ground);
	}
	[Export] public JumpingData Air {
		get => GetJumpingDataFor(Location.Air);
		set => SetJumpingDataFor(value, Location.Air);
	}
	[Export] public JumpingData Wall {
		get => GetJumpingDataFor(Location.WallNonGround);
		set => SetJumpingDataFor(value, Location.WallNonGround);
	}
	public JumpingData CurrentJumpData {
		get => GetJumpingDataFor(CurrentLocation);
		private set => SetJumpingDataFor(value, CurrentLocation);
	}

	public int NumGroundJumps {
		get => GetNumJumpsFor(Location.Ground);
		set => SetNumJumpsFor(value, Location.Ground);
	}
	public int NumAirJumps {
		get => GetNumJumpsFor(Location.Air);
		set => SetNumJumpsFor(value, Location.Air);
	}
	public int NumWallJumps {
		get => GetNumJumpsFor(Location.WallNonGround);
		set => SetNumJumpsFor(value, Location.WallNonGround);
	}
	public int CurrentNumJumps {
		get => GetNumJumpsFor(CurrentLocation);
		private set => SetNumJumpsFor(value, CurrentLocation);
	}

	public int GroundJumpCap {
		get => GetJumpCapFor(Location.Ground);
		set => SetJumpCapFor(value, Location.Ground);
	}
	public int AirJumpCap {
		get => GetJumpCapFor(Location.Air);
		set => SetJumpCapFor(value, Location.Air);
	}
	public int WallJumpCap {
		get => GetJumpCapFor(Location.WallNonGround);
		set => SetJumpCapFor(value, Location.WallNonGround);
	}
	public int CurrentJumpCap {
		get => GetJumpCapFor(CurrentLocation);
		private set => SetJumpCapFor(value, CurrentLocation);
	}
	
	public float JumpFacing { get; set; }
	public Location CurrentLocation { get; private set; } = Location.None;
	public Location CurrentLocationAfterTransition =>
		(CurrentLocation == Location.WallNonGround && CoyoteWallJump.TimeLeft > 0) ||
		 (CurrentLocation == Location.Ground && CoyoteJump.TimeLeft > 0)
			? Location.Air
			: CurrentLocation;
	public Location JumpedFrom { get; private set; } = Location.None;

	private JumpingData[] _jumpDataAll = new JumpingData[Locations.NumLocationsNotNone()];
	private int[] _numJumpsAll = new int[Locations.NumLocationsNotNone()];
	private int[] _jumpCapAll = {JumpingData.UnlimitedJumps, JumpingData.UnlimitedJumps, JumpingData.UnlimitedJumps};

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

	public void ResetNumJumps() {
		foreach (Location l in Enum.GetValues<Location>()) {
			ResetNumJumpsFor(l);
		}
	}
	
	public void ResetNumJumpsFor(Location which) {
		if (which == Location.None) return;

		SetNumJumpsFor(GetJumpingDataFor(which).NumJumps, which);
	}

	public JumpingData GetJumpingDataFor(Location forWhich) {
		return _jumpDataAll[(int)forWhich];
	}

	public void SetJumpingDataFor(JumpingData data, Location forWhich, bool updateNumJumps = true) {
		_jumpDataAll[(int)forWhich] = data;
		if (updateNumJumps) {
			SetNumJumpsFor(
				JumpingData.MinNumJumps(GetNumJumpsFor(forWhich), GetJumpingDataFor(forWhich).NumJumps), forWhich);
		}
	}

	public int GetNumJumpsFor(Location forWhich) {
		return _numJumpsAll[(int)forWhich];
	}

	public void SetNumJumpsFor(int numJumps, Location forWhich) {
		int jumpCap = GetJumpCapFor(forWhich);
		_numJumpsAll[(int)forWhich] = JumpingData.MinNumJumps(numJumps, jumpCap);
	}

	public int GetJumpCapFor(Location forWhich) {
		return _jumpCapAll[(int)forWhich];
	}

	public void SetJumpCapFor(int jumpCap, Location forWhich) {
		_jumpCapAll[(int)forWhich] = jumpCap;
		SetNumJumpsFor(GetNumJumpsFor(forWhich), forWhich);
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
		return CurrentNumJumps is JumpingData.UnlimitedJumps or > 0;
	}

	private void Jump() {
		PerformJump();

		if (CurrentNumJumps != JumpingData.UnlimitedJumps) {
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

		JumpingData lastJumpData = GetJumpingDataFor(JumpedFrom);
		if ((CurrentLocation == Location.Air && Falling.Velocity.Y < GetJumpCancelVelocityThreshold())
			|| InJumpTransitionY()) {
			Falling.SmoothlySetBaseVelocityY(lastJumpData.CancelVelocity, lastJumpData.CancelAccelTime);
		}

		EmitSignal(SignalName.CancelledJump, (int)JumpedFrom);
	}

	private float GetJumpCancelVelocityThreshold() {
		JumpingData lastJumpData = GetJumpingDataFor(JumpedFrom);
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
