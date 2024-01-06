using Godot;
using Learning.Scripts.Entity.Physics.VelocitySources;

namespace Learning.Scripts.Entity.Physics.Intermediate;

/// <summary>
///     This intermediate controls a LeftRight in order for it to be able to "snap" to a wall when
///     falling off an edge. This wall snapping is meant to make it easier to wall drag on a wall
///     after falling off an edge.
/// </summary>
public partial class WallSnapping : Node
{
    /// <summary>
    ///     A signal for when the motion for a wall snap starts.
    /// </summary>
    [Signal]
    public delegate void WallSnapStartedEventHandler();

    /// <summary>
    ///     A signal for when the motion for a wall snap stops.
    /// </summary>
    [Signal]
    public delegate void WallSnapStoppedEventHandler();

    /// <summary>
    ///     Whether this WallSnapping can currently perform wall snapping.
    /// </summary>
    [Export] private bool _enabled = true;

    private bool _inWallSnapStartWindow;
    private bool _isWallSnapping;
    private bool _wallSnapUsedUp;

    /// <summary>
    ///     Whether this WallSnapping can currently perform wall snapping.
    ///     Setting this to false will also stop wall snapping.
    /// </summary>
    private bool Enabled
    {
        get => _enabled;
        set
        {
            if (Enabled == value) return;

            if (!value) IsWallSnapping = false;

            _enabled = value;
        }
    }

    /// <summary>
    ///     The LeftRight whose velocity will be affected by the wall snapping.
    /// </summary>
    [Export]
    internal LeftRight Movement { get; private set; }

    /// <summary>
    ///     Data used to determine the movement of the wall snapping.
    /// </summary>
    [Export]
    public WallSnappingData SnapData { get; private set; }

    /// <summary>
    ///     Whether this WallSnapping is currently in a state of wall snapping.
    ///     When set to true, this will be set to false after some amount of time
    ///     (i.e., the wall snap period stopping).
    /// </summary>
    public bool IsWallSnapping
    {
        get => Enabled && _isWallSnapping;
        set
        {
            if (!Enabled || IsWallSnapping == value) return;
            _isWallSnapping = value;

            if (IsWallSnapping)
            {
                _wallSnapUsedUp = true;
                SnapData.AddModifiersTo(Movement.Air);
                WallSnapExpiry.Start();
                InWallSnapStartWindow = false;
                EmitSignal(SignalName.WallSnapStarted);
            }
            else
            {
                _wallSnapUsedUp = false;
                SnapData.RemoveModifiersFrom(Movement.Air);
                WallSnapExpiry.Stop();
                EmitSignal(SignalName.WallSnapStopped);
            }
        }
    }

    /// <summary>
    ///     Whether this WallSnapping is in a time window to start wall snapping.
    ///     When set to true, this will be set to false after some amount of time
    ///     (i.e., exiting the time window).
    /// </summary>
    public bool InWallSnapStartWindow
    {
        get => Enabled && _inWallSnapStartWindow;
        set
        {
            if (!Enabled || InWallSnapStartWindow == value) return;
            _inWallSnapStartWindow = value;

            if (InWallSnapStartWindow)
            {
                WallSnapStartWindow.Start();
                _wallSnapUsedUp = false;
            }
            else
            {
                WallSnapStartWindow.Stop();
                _wallSnapUsedUp = false;
            }
        }
    }

    private Timer WallSnapStartWindow { get; set; }
    private Timer WallSnapExpiry { get; set; }

    public override void _Ready()
    {
        WallSnapStartWindow = GetNode<Timer>(nameof(WallSnapStartWindow));
        WallSnapExpiry = GetNode<Timer>(nameof(WallSnapExpiry));

        Movement.IntendedSpeedChange += _ => WallSnapCheck();
        WallSnapStartWindow.Timeout += () => InWallSnapStartWindow = false;
        WallSnapExpiry.Timeout += () => IsWallSnapping = false;
    }

    public override void _ExitTree()
    {
        IsWallSnapping = false;
    }

    private void WallSnapCheck()
    {
        bool wasWallSnapping = IsWallSnapping;
        bool wallSnapEligible = InWallSnapStartWindow
                                && Movement.IntendedSpeedScale != 0
                                && Mathf.Sign(Movement.CurrentSpeedScale) ==
                                -Mathf.Sign(Movement.IntendedSpeedScale);

        IsWallSnapping = (wasWallSnapping, wallSnapEligible, _wallSnapUsedUp) switch
        {
            (false, true, false) => true,
            (true, false, _) => false,
            _ => IsWallSnapping
        };
    }
}