using System;
using Godot;
using Learning.Scripts.Entity.Physics.VelocitySources;

namespace Learning.Scripts.Entity.Physics.Intermediate;

public partial class Jumping : Node
{
    [Signal]
    public delegate void CancelledJumpEventHandler(Location jumpWasFrom);

    [Signal]
    public delegate void JumpedEventHandler(Location from);

    private int[] _jumpCapAll =
        {JumpingData.UnlimitedJumps, JumpingData.UnlimitedJumps, JumpingData.UnlimitedJumps};

    private JumpingData[] _jumpDataAll = new JumpingData[Locations.NumLocationsNotNone()];
    private Tween _jumpTweenX;

    private Tween _jumpTweenY;

    private float _lastWallDirection;
    private int[] _numJumpsAll = new int[Locations.NumLocationsNotNone()];
    [Export] private bool Enabled { get; set; } = true;

    [Export] internal Falling Falling { get; private set; }
    [Export] internal LeftRight LeftRight { get; private set; }

    [Export] private bool GroundJumpingEnabled { get; set; } = true;
    [Export] private bool AirJumpingEnabled { get; set; } = true;
    [Export] private bool WallJumpingEnabled { get; set; } = true;

    [Export] private bool CoyoteGroundJumpEnabled { get; set; } = true;
    [Export] private bool GroundJumpBufferEnabled { get; set; } = true;
    [Export] private bool CoyoteWallJumpEnabled { get; set; } = true;
    [Export] private bool WallJumpBufferEnabled { get; set; } = true;

    [Export]
    public JumpingData Ground
    {
        get => GetJumpingDataFor(Location.Ground);
        private set => SetJumpingDataFor(value, Location.Ground);
    }

    [Export]
    public JumpingData Air
    {
        get => GetJumpingDataFor(Location.Air);
        private set => SetJumpingDataFor(value, Location.Air);
    }

    [Export]
    public JumpingData Wall
    {
        get => GetJumpingDataFor(Location.WallNonGround);
        private set => SetJumpingDataFor(value, Location.WallNonGround);
    }

    public JumpingData CurrentJumpData
    {
        get => GetJumpingDataFor(CurrentLocation);
        private set => SetJumpingDataFor(value, CurrentLocation);
    }

    public int NumGroundJumps
    {
        get => GetNumJumpsFor(Location.Ground);
        set => SetNumJumpsFor(value, Location.Ground);
    }

    public int NumAirJumps
    {
        get => GetNumJumpsFor(Location.Air);
        set => SetNumJumpsFor(value, Location.Air);
    }

    public int NumWallJumps
    {
        get => GetNumJumpsFor(Location.WallNonGround);
        set => SetNumJumpsFor(value, Location.WallNonGround);
    }

    public int CurrentNumJumps
    {
        get => GetNumJumpsFor(CurrentLocation);
        private set => SetNumJumpsFor(value, CurrentLocation);
    }

    public int GroundJumpCap
    {
        get => GetJumpCapFor(Location.Ground);
        set => SetJumpCapFor(value, Location.Ground);
    }

    public int AirJumpCap
    {
        get => GetJumpCapFor(Location.Air);
        set => SetJumpCapFor(value, Location.Air);
    }

    public int WallJumpCap
    {
        get => GetJumpCapFor(Location.WallNonGround);
        set => SetJumpCapFor(value, Location.WallNonGround);
    }

    public int CurrentJumpCap
    {
        get => GetJumpCapFor(CurrentLocation);
        private set => SetJumpCapFor(value, CurrentLocation);
    }

    private bool CanJump =>
        IsJumpingEnabledFor(CurrentLocation) &&
        CurrentNumJumps is JumpingData.UnlimitedJumps or > 0;

    public float JumpFacing { get; set; }
    public Location CurrentLocation { get; private set; } = Location.None;

    public Location CurrentLocationAfterTransition =>
        (CurrentLocation == Location.WallNonGround && CoyoteWallJump.TimeLeft > 0) ||
        (CurrentLocation == Location.Ground && CoyoteJump.TimeLeft > 0)
            ? Location.Air
            : CurrentLocation;

    public Location JumpedFrom { get; private set; } = Location.None;

    public bool InJumpTransitionY => _jumpTweenY != null && _jumpTweenY.IsValid();

    public bool InJumpTransitionX => _jumpTweenX != null && _jumpTweenX.IsValid();

    private float JumpCancelVelocityThreshold
    {
        get
        {
            JumpingData lastJumpData = GetJumpingDataFor(JumpedFrom);
            float extraVelocity =
                Falling.Gravity * Falling.GravityScale * lastJumpData.CancelAccelTime;
            return lastJumpData.CancelVelocity - extraVelocity;
        }
    }

    private Timer CoyoteJump { get; set; }
    private Timer JumpBuffer { get; set; }
    private Timer CoyoteWallJump { get; set; }
    private Timer WallJumpBuffer { get; set; }

    public override void _Ready()
    {
        CoyoteJump = GetNode<Timer>(nameof(CoyoteJump));
        JumpBuffer = GetNode<Timer>(nameof(JumpBuffer));
        CoyoteWallJump = GetNode<Timer>(nameof(CoyoteWallJump));
        WallJumpBuffer = GetNode<Timer>(nameof(WallJumpBuffer));

        CoyoteJump.Timeout += () => CurrentLocation = Location.Air;
        CoyoteWallJump.Timeout += () => CurrentLocation = Location.Air;

        Ground.ModifiersUpdated += () => UpdateNumJumpsFor(Location.Ground);
        Air.ModifiersUpdated += () => UpdateNumJumpsFor(Location.Air);
        Wall.ModifiersUpdated += () => UpdateNumJumpsFor(Location.WallNonGround);

        ResetNumJumps();
    }

    public int GetJumpCapFor(Location forWhich)
    {
        return _jumpCapAll[(int)forWhich];
    }

    public void SetJumpCapFor(int jumpCap, Location forWhich)
    {
        _jumpCapAll[(int)forWhich] = jumpCap;
        SetNumJumpsFor(GetNumJumpsFor(forWhich), forWhich);
    }

    public int GetNumJumpsFor(Location forWhich)
    {
        return _numJumpsAll[(int)forWhich];
    }

    public void SetNumJumpsFor(int numJumps, Location forWhich)
    {
        int jumpCap = GetJumpCapFor(forWhich);
        _numJumpsAll[(int)forWhich] = JumpingData.MinNumJumps(numJumps, jumpCap);
    }

    public void ResetNumJumps()
    {
        foreach (Location l in Enum.GetValues<Location>()) ResetNumJumpsFor(l);
    }

    public void ResetNumJumpsFor(Location which)
    {
        if (which == Location.None) return;

        SetNumJumpsFor(GetJumpingDataFor(which).NumJumps, which);
    }

    private void UpdateNumJumpsFor(Location which)
    {
        SetNumJumpsFor(
            JumpingData.MinNumJumps(GetNumJumpsFor(which), GetJumpingDataFor(which).NumJumps),
            which);
    }

    public JumpingData GetJumpingDataFor(Location forWhich)
    {
        return _jumpDataAll[(int)forWhich];
    }

    private void SetJumpingDataFor(JumpingData data, Location forWhich)
    {
        _jumpDataAll[(int)forWhich] = data;
        UpdateNumJumpsFor(forWhich);
    }

    public bool IsJumpingEnabledFor(Location forWhich)
    {
        // can't use array here since array initialization to true doesn't show up as true in Godot editor
        return Enabled && forWhich switch
        {
            Location.Ground => GroundJumpingEnabled,
            Location.Air => AirJumpingEnabled,
            Location.WallNonGround => WallJumpingEnabled,
            _ => false
        };
    }

    public void AttemptJump()
    {
        if (CanJump)
        {
            Jump();
        }
        else if (IsJumpingEnabledFor(CurrentLocation))
        {
            if (GroundJumpBufferEnabled) JumpBuffer.Start();
            if (WallJumpBufferEnabled) WallJumpBuffer.Start();
        }
    }

    private void AttemptBufferedGroundJump()
    {
        if (CanJump && JumpBuffer.TimeLeft > 0) Jump();
    }

    private void AttemptBufferedWallJump()
    {
        if (CanJump && WallJumpBuffer.TimeLeft > 0) Jump();
    }

    private void Jump()
    {
        PerformJump();

        if (CurrentNumJumps != JumpingData.UnlimitedJumps) CurrentNumJumps--;

        JumpedFrom = CurrentLocation;
        EmitSignal(SignalName.Jumped, (int)JumpedFrom);
        if (CoyoteJump.TimeLeft > 0 || CoyoteWallJump.TimeLeft > 0) CurrentLocation = Location.Air;

        JumpBuffer.Stop();
        WallJumpBuffer.Stop();
        CoyoteJump.Stop();
        CoyoteWallJump.Stop();
    }

    private void PerformJump()
    {
        if (CurrentJumpData.Velocity.Y != 0)
        {
            (_jumpTweenY, PropertyTweener t)
                = Falling.SmoothlySetBaseVelocityY(CurrentJumpData.Velocity.Y,
                    CurrentJumpData.AccelTimeY);
            t.SetTrans(Tween.TransitionType.Expo);
        }

        if (CurrentJumpData.Velocity.X != 0)
        {
            (_jumpTweenX, PropertyTweener t)
                = LeftRight.SetIntendedSpeed(CurrentJumpData.Velocity.X * JumpFacing,
                    CurrentJumpData.AccelTimeX);
            t.SetTrans(Tween.TransitionType.Expo);
        }
    }

    public void JumpCancel()
    {
        JumpBuffer.Stop();
        WallJumpBuffer.Stop();

        if (JumpedFrom == Location.None) return;

        bool inNormalJump = CurrentLocation == Location.Air &&
                            Falling.Velocity.Y < JumpCancelVelocityThreshold;
        if ((inNormalJump || InJumpTransitionY) && !Falling.StoppingDueToCeilingHit)
        {
            JumpingData lastJumpData = GetJumpingDataFor(JumpedFrom);
            Falling.SmoothlySetBaseVelocityY(lastJumpData.CancelVelocity,
                lastJumpData.CancelAccelTime);
        }

        EmitSignal(SignalName.CancelledJump, (int)JumpedFrom);
    }

    public void TransitionToGround()
    {
        CoyoteJump.Stop();
        CoyoteWallJump.Stop();

        CurrentLocation = Location.Ground;
        AttemptBufferedGroundJump();
    }

    public void TransitionToAir(bool immediately = false)
    {
        if (!immediately
            && CoyoteGroundJumpEnabled
            && Falling.Velocity.Y >= 0
            && CurrentLocation == Location.Ground)
            CoyoteJump.Start();
        else if (!immediately
                 && CoyoteWallJumpEnabled
                 && Falling.Velocity.Y >= 0
                 && CurrentLocation == Location.WallNonGround)
            CoyoteWallJump.Start();
        else
            CurrentLocation = Location.Air;
    }

    public void TransitionToWall(float wallDirection)
    {
        CoyoteJump.Stop();
        CoyoteWallJump.Stop();

        CurrentLocation = Location.WallNonGround;
        JumpFacing = _lastWallDirection = wallDirection;
        AttemptBufferedWallJump();
    }
}