using System;
using Godot;
using Learning.Scripts.Entity.Physics.VelocitySources;

namespace Learning.Scripts.Entity.Physics.Intermediate;

/// <summary>
/// This intermediate controls a Falling in order to make an entity move as if it's jumping.
/// It can also control a LeftRight to give the entity the horizontal movement for a wall jump.
/// </summary>
public partial class Jumping : Node
{
    /// <summary>
    /// Signal for when a jump gets cancelled.
    /// </summary>
    [Signal]
    public delegate void CancelledJumpEventHandler(Location jumpWasFrom);

    /// <summary>
    /// Signal for when this Jumping performs a jump.
    /// </summary>
    /// <param name="from"> </param>
    [Signal]
    public delegate void JumpedEventHandler(Location from);

    private int[] _jumpCapAll =
        {JumpingData.UnlimitedJumps, JumpingData.UnlimitedJumps, JumpingData.UnlimitedJumps};

    private JumpingData[] _jumpDataAll = new JumpingData[Locations.NumLocationsNotNone()];
    private Tween _jumpTweenX;

    private Tween _jumpTweenY;

    private float _lastWallDirection;
    private int[] _numJumpsAll = new int[Locations.NumLocationsNotNone()];
    
    /// <summary>
    /// If this is false, then attempts to jump will not result in a jump.
    /// </summary>
    [Export] private bool Enabled { get; set; } = true;

    /// <summary>
    /// The Falling that is affected by this Jumping's jumps.
    /// </summary>
    [Export] internal Falling Falling { get; private set; }
    
    /// <summary>
    /// The LeftRight that is affected by this Jumping's wall jumps.
    /// </summary>
    [Export] internal LeftRight LeftRight { get; private set; }

    /// <summary>
    /// If this is false, the attempts to do a ground jump will not result in a jump.
    /// </summary>
    [Export] private bool GroundJumpingEnabled { get; set; } = true;
    
    /// <summary>
    /// If this is false, the attempts to do an air jump will not result in a jump.
    /// </summary>
    [Export] private bool AirJumpingEnabled { get; set; } = true;
    
    /// <summary>
    /// If this is false, the attempts to do wall jump will not result in a jump.
    /// </summary>
    [Export] private bool WallJumpingEnabled { get; set; } = true;

    /// <summary>
    /// Whether "coyote jumps" off the ground (can ground jump a short amount of time after
    /// falling off a ledge) are enabled.
    /// </summary>
    [Export] private bool CoyoteGroundJumpEnabled { get; set; } = true;
    
    /// <summary>
    /// Whether "buffered jumps" off the ground (inputting a jump, a short amount of time
    /// before hitting the ground, registering as a jump) are enabled.
    /// </summary>
    [Export] private bool GroundJumpBufferEnabled { get; set; } = true;
    
    /// <summary>
    /// Whether "coyote jumps" off a wall (can wall jump a short amount of time after
    /// coming off a wall) are enabled.
    /// </summary>
    [Export] private bool CoyoteWallJumpEnabled { get; set; } = true;
    
    /// <summary>
    /// Whether "buffered jumps" off the ground (inputting a jump, a short amount of time
    /// before hitting the ground, registering as a jump) are enabled.
    /// </summary>
    [Export] private bool WallJumpBufferEnabled { get; set; } = true;
    
    /// <summary>
    /// Data for a ground jump's movement.
    /// </summary>
    [Export]
    public JumpingData Ground
    {
        get => GetJumpingDataFor(Location.Ground);
        private set => SetJumpingDataFor(value, Location.Ground);
    }

    /// <summary>
    /// Data for an air jump's movement.
    /// </summary>
    [Export]
    public JumpingData Air
    {
        get => GetJumpingDataFor(Location.Air);
        private set => SetJumpingDataFor(value, Location.Air);
    }

    /// <summary>
    /// Data for a wall jump's movement.
    /// </summary>
    [Export]
    public JumpingData Wall
    {
        get => GetJumpingDataFor(Location.WallNonGround);
        private set => SetJumpingDataFor(value, Location.WallNonGround);
    }

    /// <summary>
    /// The data for the jump movement for the current location.
    /// </summary>
    public JumpingData CurrentJumpData
    {
        get => GetJumpingDataFor(CurrentLocation);
        private set => SetJumpingDataFor(value, CurrentLocation);
    }

    /// <summary>
    /// The current number of ground jumps remaining.
    /// A value of zero means that a ground jump cannot be performed.
    /// </summary>
    public int NumGroundJumps
    {
        get => GetNumJumpsFor(Location.Ground);
        set => SetNumJumpsFor(value, Location.Ground);
    }

    /// <summary>
    /// The current number of air jumps remaining.
    /// A value of zero means that an air jump cannot be performed.
    /// </summary>
    public int NumAirJumps
    {
        get => GetNumJumpsFor(Location.Air);
        set => SetNumJumpsFor(value, Location.Air);
    }

    /// <summary>
    /// The current number of wall jumps remaining.
    /// A value of zero means that a wall jump cannot be performed.
    /// </summary>
    public int NumWallJumps
    {
        get => GetNumJumpsFor(Location.WallNonGround);
        set => SetNumJumpsFor(value, Location.WallNonGround);
    }

    /// <summary>
    /// The current number of jumps remaining for the current location.
    /// A value of zero means that a jump cannot be performed in the current location.
    /// </summary>
    public int CurrentNumJumps
    {
        get => GetNumJumpsFor(CurrentLocation);
        private set => SetNumJumpsFor(value, CurrentLocation);
    }

    /// <summary>
    /// The (soft) cap on ground jumps; resetting the number of ground jumps causes
    /// the current number of ground jumps to go to this value.
    /// </summary>
    public int GroundJumpCap
    {
        get => GetJumpCapFor(Location.Ground);
        set => SetJumpCapFor(value, Location.Ground);
    }

    /// <summary>
    /// The (soft) cap on air jumps; resetting the number of air jumps causes
    /// the current number of air jumps to go to this value.
    /// </summary>
    public int AirJumpCap
    {
        get => GetJumpCapFor(Location.Air);
        set => SetJumpCapFor(value, Location.Air);
    }

    /// <summary>
    /// The (soft) cap on wall jumps; resetting the number of wall jumps causes
    /// the current number of wall jumps to go to this value.
    /// </summary>
    public int WallJumpCap
    {
        get => GetJumpCapFor(Location.WallNonGround);
        set => SetJumpCapFor(value, Location.WallNonGround);
    }

    /// <summary>
    /// The (soft) cap on jumps in the current location; resetting the number of jumps causes
    /// the current number of jumps to go to this value.
    /// </summary>
    public int CurrentJumpCap
    {
        get => GetJumpCapFor(CurrentLocation);
        private set => SetJumpCapFor(value, CurrentLocation);
    }

    private bool CanJump =>
        IsJumpingEnabledFor(CurrentLocation) &&
        CurrentNumJumps is JumpingData.UnlimitedJumps or > 0;

    /// <summary>
    /// The x direction to go when performing a jump that has horizontal movement
    /// (e.g. a wall jump). Positive values go right; negative values go left.
    /// </summary>
    public float JumpFacing { get; set; }
    
    /// <summary>
    /// Which location this Jumping is in. Affects the type of jump done when attempting jumps.
    /// </summary>
    public Location CurrentLocation { get; private set; } = Location.None;

    /// <summary>
    /// The current location, after coyote transitions are done.
    /// </summary>
    public Location CurrentLocationAfterTransition =>
        (CurrentLocation == Location.WallNonGround && CoyoteWallJump.TimeLeft > 0) ||
        (CurrentLocation == Location.Ground && CoyoteJump.TimeLeft > 0)
            ? Location.Air
            : CurrentLocation;

    /// <summary>
    /// Which location the previous jump was performed in.
    /// </summary>
    public Location JumpedFrom { get; private set; } = Location.None;

    /// <summary>
    /// Whether the Falling's y velocity is currently changing due to a jump.
    /// </summary>
    public bool InJumpTransitionY => _jumpTweenY != null && _jumpTweenY.IsValid();

    /// <summary>
    /// Whether the LeftRight's x velocity is currently changing due to a jump.
    /// </summary>
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

        Ground.ModifierUpdated += (_, _) => UpdateNumJumpsFor(Location.Ground);
        Air.ModifierUpdated += (_, _) => UpdateNumJumpsFor(Location.Air);
        Wall.ModifierUpdated += (_, _) => UpdateNumJumpsFor(Location.WallNonGround);

        ResetNumJumps();
    }

    /// <returns>The (soft) cap on the number of jumps for the given location.</returns>
    public int GetJumpCapFor(Location forWhich)
    {
        return _jumpCapAll[(int)forWhich];
    }

    /// <summary>
    /// Sets the (soft) cap on the number of jumps for the given location.
    /// Also caps the current number of jumps for the given location to the cap value.
    /// </summary>
    /// <param name="jumpCap">The number of jumps to cap at</param>
    /// <param name="forWhich">The location to set the jump cap for</param>
    public void SetJumpCapFor(int jumpCap, Location forWhich)
    {
        _jumpCapAll[(int)forWhich] = jumpCap;
        SetNumJumpsFor(GetNumJumpsFor(forWhich), forWhich);
    }

    /// <returns>The current number of jumps for the given location.</returns>
    public int GetNumJumpsFor(Location forWhich)
    {
        return _numJumpsAll[(int)forWhich];
    }

    /// <summary>
    /// Sets the current number of jumps for the given location.
    /// </summary>
    /// <param name="numJumps">The number of jumps</param>
    /// <param name="forWhich">The location to set the number of jumps of</param>
    public void SetNumJumpsFor(int numJumps, Location forWhich)
    {
        int jumpCap = GetJumpCapFor(forWhich);
        _numJumpsAll[(int)forWhich] = JumpingData.MinNumJumps(numJumps, jumpCap);
    }

    /// <summary>
    /// Resets the number of jumps for all locations, i.e., sets the number of jumps for each
    /// location to the respective jump cap.
    /// </summary>
    public void ResetNumJumps()
    {
        foreach (Location l in Enum.GetValues<Location>()) ResetNumJumpsFor(l);
    }

    /// <summary>
    /// Resets the number of jumps for the current location, i.e., sets the number of jumps
    /// to the respective jump cap.
    /// </summary>
    /// <param name="which">The location to reset the number of jumps for</param>
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

    /// <returns>The data for the jump movement for the given location.</returns>
    public JumpingData GetJumpingDataFor(Location forWhich)
    {
        return _jumpDataAll[(int)forWhich];
    }

    private void SetJumpingDataFor(JumpingData data, Location forWhich)
    {
        _jumpDataAll[(int)forWhich] = data;
        UpdateNumJumpsFor(forWhich);
    }

    /// <returns>Whether jumping is enabled for the given location.</returns>
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

    /// <summary>
    /// Performs a jump for the current location,
    /// if currently able to perform a jump in the given location.
    /// </summary>
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

    /// <summary>
    /// Cancels the current jump, if able. Cancelling a jump means that the y velocity
    /// of the Falling smoothly goes to the current jump cancel velocity,
    /// over the cancel acceleration time for the location jumped from.
    /// A jump can be cancelled if the projected y velocity after the cancel acceleration time
    /// (i.e., what the y velocity would've been if the jump wasn't cancelled
    /// and [cancel acceleration time] had passed) is greater than the jump cancel velocity.
    /// </summary>
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

    /// <summary>
    /// Changes the current location to Location.Ground and performs a buffered ground jump
    /// if able.
    /// </summary>
    public void TransitionToGround()
    {
        CoyoteJump.Stop();
        CoyoteWallJump.Stop();

        CurrentLocation = Location.Ground;
        AttemptBufferedGroundJump();
    }

    /// <summary>
    /// Changes the current location to Location.Air after coyote timings.
    /// </summary>
    /// <param name="immediately">If true, ignores coyote timings</param>
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

    /// <summary>
    /// Changes the current location to Location.WallNonGround and performs a buffered wall jump
    /// if able.
    /// </summary>
    /// <param name="wallDirection">The x component of the wall normal. The wall being to the left
    /// of the entity corresponds to a positive value.</param>
    public void TransitionToWall(float wallDirection)
    {
        CoyoteJump.Stop();
        CoyoteWallJump.Stop();

        CurrentLocation = Location.WallNonGround;
        JumpFacing = _lastWallDirection = wallDirection;
        AttemptBufferedWallJump();
    }
}