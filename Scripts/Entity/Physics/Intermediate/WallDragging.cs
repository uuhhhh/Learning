using Godot;
using Learning.Scripts.Entity.Physics.VelocitySources;

namespace Learning.Scripts.Entity.Physics.Intermediate;

/// <summary>
///     This intermediate controls a Falling in order for an entity to move as if it's dragging down
///     a wall instead of falling.
/// </summary>
public partial class WallDragging : Node
{
    /// <summary>
    ///     Signal for when this WallDragging starts wall dragging.
    /// </summary>
    [Signal]
    public delegate void StartedDraggingEventHandler();

    /// <summary>
    ///     Signal for when this WallDragging starts validly touching a wall.
    /// </summary>
    [Signal]
    public delegate void StartedValidWallTouchingEventHandler();

    /// <summary>
    ///     Signal for when this WallDragging stops wall dragging.
    /// </summary>
    [Signal]
    public delegate void StoppedDraggingEventHandler();

    /// <summary>
    ///     Signal for when this WallDragging stops validly touching a wall.
    /// </summary>
    [Signal]
    public delegate void StoppedValidWallTouchingEventHandler();

    /// <summary>
    ///     Whether wall dragging is currently enabled for this WallDragging.
    /// </summary>
    [Export] private bool _enabled = true;

    private bool _isDragging;
    private bool _isValidWallTouching;
    private WallDraggingData _wall;

    /// <summary>
    ///     Whether wall dragging is currently enabled for this WallDragging.
    ///     Setting this to false will stop wall dragging.
    /// </summary>
    private bool Enabled
    {
        get => _enabled;
        set
        {
            if (Enabled == value) return;

            if (!value) IsDragging = false;

            _enabled = value;
        }
    }

    /// <summary>
    ///     The Falling that is affected by this WallDragging's wall dragging
    /// </summary>
    [Export]
    internal Falling Falling { get; private set; }

    /// <summary>
    ///     Data used for the wall dragging movement.
    /// </summary>
    [Export]
    public WallDraggingData Wall
    {
        get => _wall;
        private set
        {
            _wall = value;
            WallDataUpdated();
        }
    }

    /// <summary>
    ///     Whether this WallDragging is in a state of wall dragging.
    /// </summary>
    public bool IsDragging
    {
        get => Enabled && _isDragging;
        private set
        {
            if (!Enabled || value == IsDragging) return;

            _isDragging = value;
            if (IsDragging)
            {
                Wall.AddModifiersTo(Falling.FallData);
                EmitSignal(SignalName.StartedDragging);
            }
            else
            {
                Wall.RemoveModifiersFrom(Falling.FallData);
                EmitSignal(SignalName.StoppedDragging);
            }
        }
    }

    /// <summary>
    ///     Whether this WallDragging is in a state of validly touching a wall.
    ///     (when validly touching a wall, this WallDragging will begin wall dragging when the Falling's
    ///     y velocity passes the drag threshold).
    /// </summary>
    public bool ValidWallTouching
    {
        get => Enabled && _isValidWallTouching;
        set
        {
            if (!Enabled || value == _isValidWallTouching) return;

            _isValidWallTouching = value;

            if (!_isValidWallTouching) IsDragging = false;
            EmitSignal(ValidWallTouching
                ? SignalName.StartedValidWallTouching
                : SignalName.StoppedValidWallTouching);
        }
    }

    public override void _Ready()
    {
        Wall.ModifierUpdated += (_, _) => WallDataUpdated();
    }

    private void WallDataUpdated()
    {
        if (IsNodeReady()) DraggingCheck();
    }

    public override void _PhysicsProcess(double delta)
    {
        DraggingCheck();
    }

    /// <summary>
    ///     Start dragging if the necessary conditions are met: being in a state of valid wall touching,
    ///     and the Falling's y velocity is past the velocity drag threshold.
    /// </summary>
    internal void DraggingCheck()
    {
        IsDragging = ValidWallTouching &&
                     Falling.VelocityAfterTransition.Y >= Wall.VelocityDragThreshold;
    }
}