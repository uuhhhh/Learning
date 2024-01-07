using Godot;

namespace Learning.Scripts.Entity.Physics;

/// <summary>
///     A CharacterBody2D with additional behaviors and signals.
/// </summary>
public partial class KinematicComp : CharacterBody2D
{
    // Whenever a new signal is added here, be sure the change is reflected in DefaultPhys also

    /// <summary>
    ///     A signal for when this body is no longer touching the ceiling.
    /// </summary>
    [Signal]
    public delegate void BecomeOffCeilingEventHandler(KinematicComp state);

    /// <summary>
    ///     A signal for when this body is no longer touching the floor.
    /// </summary>
    [Signal]
    public delegate void BecomeOffFloorEventHandler(KinematicComp state);

    /// <summary>
    ///     A signal for when this body is no longer touching any walls.
    /// </summary>
    [Signal]
    public delegate void BecomeOffWallEventHandler(KinematicComp state);

    /// <summary>
    ///     A signal for when this body begins touching a ceiling.
    /// </summary>
    [Signal]
    public delegate void BecomeOnCeilingEventHandler(KinematicComp state);

    /// <summary>
    ///     A signal for when this body begins touching a floor.
    /// </summary>
    [Signal]
    public delegate void BecomeOnFloorEventHandler(KinematicComp state);

    /// <summary>
    ///     A signal for when this body begins touching a wall.
    /// </summary>
    [Signal]
    public delegate void BecomeOnWallEventHandler(KinematicComp state);

    /// <summary>
    ///     A signal for when this body starts moving (a non-negligible amount) in the other direction
    ///     in the x-axis.
    /// </summary>
    [Signal]
    public delegate void DirectionChangeXEventHandler(KinematicComp state, float newDirection);

    /// <summary>
    ///     A signal for when this body starts moving (a non-negligible amount) in the other direction
    ///     in the y-axis.
    /// </summary>
    [Signal]
    public delegate void DirectionChangeYEventHandler(KinematicComp state, float newDirection);

    private bool _increasingX, _increasingY;

    private Vector2 _mostExtremePosition;

    /// <summary>
    ///     The distance threshold when moving in the opposite direction,
    ///     to register it as a direction change.
    /// </summary>
    [Export]
    private float DirectionChangeEpsilon { get; set; } = .01f;

    public override void _Ready()
    {
        base._Ready();
        _mostExtremePosition = Position;

        SignalInitialState();
    }

    private void SignalInitialState()
    {
        EmitSignal(IsOnCeiling() ? SignalName.BecomeOnCeiling : SignalName.BecomeOffCeiling, this);
        EmitSignal(IsOnWall() ? SignalName.BecomeOnWall : SignalName.BecomeOffWall, this);
        EmitSignal(IsOnFloor() ? SignalName.BecomeOnFloor : SignalName.BecomeOffFloor, this);
    }

    /// <summary>
    ///     CharacterBody2D's MoveAndSlide, but with additional behavior to make
    ///     KinematicComp's additional signals operate.
    /// </summary>
    /// <returns>Whether there was a collision with the MoveAndSlide</returns>
    public virtual bool MoveAndSlideWithStatusChanges()
    {
        bool wasOnFloor = IsOnFloor(), wasOnCeiling = IsOnCeiling(), wasOnWall = IsOnWall();

        bool collided = MoveAndSlide();

        CheckFloorStatusChange(wasOnFloor);
        CheckCeilingStatusChange(wasOnCeiling);
        CheckWallStatusChange(wasOnWall);

        CheckDirectionChange();

        return collided;
    }

    private void CheckFloorStatusChange(bool wasOnFloor)
    {
        switch (wasOnFloor, IsOnFloor())
        {
            case (true, false):
                EmitSignal(SignalName.BecomeOffFloor, this);
                break;
            case (false, true):
                EmitSignal(SignalName.BecomeOnFloor, this);
                break;
        }
    }

    private void CheckCeilingStatusChange(bool wasOnCeiling)
    {
        switch (wasOnCeiling, IsOnCeiling())
        {
            case (true, false):
                EmitSignal(SignalName.BecomeOffCeiling, this);
                break;
            case (false, true):
                EmitSignal(SignalName.BecomeOnCeiling, this);
                break;
        }
    }

    private void CheckWallStatusChange(bool wasOnWall)
    {
        switch (wasOnWall, IsOnWall())
        {
            case (true, false):
                EmitSignal(SignalName.BecomeOffWall, this);
                break;
            case (false, true):
                EmitSignal(SignalName.BecomeOnWall, this);
                break;
        }
    }

    private void CheckDirectionChange()
    {
        _mostExtremePosition.X = _increasingX
            ? Mathf.Max(Position.X, _mostExtremePosition.X)
            : Mathf.Min(Position.X, _mostExtremePosition.X);
        bool turnedAroundX =
            Mathf.Abs(Position.X - _mostExtremePosition.X) > DirectionChangeEpsilon;

        if (turnedAroundX)
        {
            EmitSignal(SignalName.DirectionChangeX, this, Position.X - _mostExtremePosition.X);
            _increasingX = !_increasingX;
            _mostExtremePosition.X = Position.X;
        }

        _mostExtremePosition.Y = _increasingY
            ? Mathf.Max(Position.Y, _mostExtremePosition.Y)
            : Mathf.Min(Position.Y, _mostExtremePosition.Y);
        bool turnedAroundY =
            Mathf.Abs(Position.Y - _mostExtremePosition.Y) > DirectionChangeEpsilon;

        if (turnedAroundY)
        {
            EmitSignal(SignalName.DirectionChangeY, this, Position.Y - _mostExtremePosition.Y);
            _increasingY = !_increasingY;
            _mostExtremePosition.Y = Position.Y;
        }
    }

    /// <summary>
    ///     Register this KinematicComp's signals with the given DefaultPhys,
    ///     if it doesn't not want to be linked.
    /// </summary>
    /// <param name="phys">The DefaultPhys to try to link</param>
    public void LinkDefaultPhys(DefaultPhys phys)
    {
        if (!phys.DoNotLink) phys.Link(this);
    }

    /// <summary>
    ///     Performs extra initialization the given DefaultPhys,
    ///     if it doesn't not want to be extra initialized.
    /// </summary>
    /// <param name="phys">The DefaultPhys to try to do extra initialization on</param>
    public void ExtraInitDefaultPhys(DefaultPhys phys)
    {
        if (!phys.DoNotCallExtraInit) phys.ExtraInit(this);
    }
}