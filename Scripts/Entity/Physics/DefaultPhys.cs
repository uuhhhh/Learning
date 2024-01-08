using Godot;

namespace Learning.Scripts.Entity.Physics;

/// <summary>
///     Meant for defining "default physics behaviors" of VelocitySources and intermediates
///     (i.e., what should the entity do by default when a given thing happens with the KinematicComp).
/// </summary>
public abstract partial class DefaultPhys : Node
{
    /// <summary>
    ///     Set this to true for KinematicComps to not call Link with this DefaultPhys.
    /// </summary>
    /// <see cref="KinematicComp.LinkDefaultPhys" />
    [Export]
    internal bool DoNotLink { get; set; }

    /// <summary>
    ///     Set this to true for KinematicComps to not call ExtraInit with this DefaultPhys.
    /// </summary>
    /// <see cref="KinematicComp.ExtraInitDefaultPhys" />
    [Export]
    internal bool DoNotCallExtraInit { get; set; }

    /// <summary>
    ///     Register the given KinematicComp's signals with this DefaultPhys, so this DefaultPhys'
    ///     methods are called when the KinematicComp's signals are emitted.
    /// </summary>
    internal void Link(KinematicComp physics)
    {
        physics.BecomeOnFloor += OnBecomeOnFloor;
        physics.BecomeOffFloor += OnBecomeOffFloor;
        physics.BecomeOnCeiling += OnBecomeOnCeiling;
        physics.BecomeOffCeiling += OnBecomeOffCeiling;
        physics.BecomeOnWall += OnBecomeOnWall;
        physics.BecomeOffWall += OnBecomeOffWall;
        physics.DirectionChangeX += OnDirectionChangeX;
        physics.DirectionChangeY += OnDirectionChangeY;
    }

    /// <summary>
    ///     Any extra initialization needed by the implementer, based on the given KinematicComp.
    /// </summary>
    internal virtual void ExtraInit(KinematicComp physics)
    {
    }

    /// <summary>
    ///     Called whenever the given (linked) physics goes from
    ///     not being on floor to being on a floor.
    /// </summary>
    internal virtual void OnBecomeOnFloor(KinematicComp physics)
    {
    }

    /// <summary>
    ///     Called whenever the given (linked) physics goes from
    ///     being on floor to not being on a floor.
    /// </summary>
    internal virtual void OnBecomeOffFloor(KinematicComp physics)
    {
    }

    /// <summary>
    ///     Called whenever the given (linked) physics goes from
    ///     not being on ceiling to being on a ceiling.
    /// </summary>
    internal virtual void OnBecomeOnCeiling(KinematicComp physics)
    {
    }

    /// <summary>
    ///     Called whenever the given (linked) physics goes from
    ///     being on ceiling to not being on a ceiling.
    /// </summary>
    internal virtual void OnBecomeOffCeiling(KinematicComp physics)
    {
    }

    /// <summary>
    ///     Called whenever the given (linked) physics goes from
    ///     not being on wall to being on a wall.
    /// </summary>
    internal virtual void OnBecomeOnWall(KinematicComp physics)
    {
    }

    /// <summary>
    ///     Called whenever the given (linked) physics goes from
    ///     being on wall to not being on a wall.
    /// </summary>
    internal virtual void OnBecomeOffWall(KinematicComp physics)
    {
    }

    /// <summary>
    ///     Called whenever the given (linked) physics goes from moving in one x direction to the
    ///     other (or moving from one x direction to stopping to moving the other direction).
    ///     Note that the movement in the other direction has to be larger than some very small
    ///     value for this method to be called.
    /// </summary>
    /// <param name="physics">The (linked) physics that started moving
    /// in the other direction</param>
    /// <param name="newDirection">The sign of this float indicates the sign of the x direction
    /// that the KinematicComp is now moving</param>
    internal virtual void OnDirectionChangeX(KinematicComp physics, float newDirection)
    {
    }

    /// <summary>
    ///     Called whenever the given (linked) physics goes from moving in one y direction to the
    ///     other (or moving from one y direction to stopping to moving the other direction).
    ///     Note that the movement in the other direction has to be larger than some very small
    ///     value for this method to be called.
    /// </summary>
    /// <param name="physics">The (linked) physics that started moving
    /// in the other direction</param>
    /// <param name="newDirection">The sign of this float indicates the sign of the y direction
    /// that the KinematicComp is now moving</param>
    internal virtual void OnDirectionChangeY(KinematicComp physics, float newDirection)
    {
    }
}