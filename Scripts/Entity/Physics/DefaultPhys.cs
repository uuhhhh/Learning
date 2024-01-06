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

    internal virtual void OnBecomeOnFloor(KinematicComp physics)
    {
    }

    internal virtual void OnBecomeOffFloor(KinematicComp physics)
    {
    }

    internal virtual void OnBecomeOnCeiling(KinematicComp physics)
    {
    }

    internal virtual void OnBecomeOffCeiling(KinematicComp physics)
    {
    }

    internal virtual void OnBecomeOnWall(KinematicComp physics)
    {
    }

    internal virtual void OnBecomeOffWall(KinematicComp physics)
    {
    }

    internal virtual void OnDirectionChangeX(KinematicComp physics, float newDirection)
    {
    }

    internal virtual void OnDirectionChangeY(KinematicComp physics, float newDirection)
    {
    }
}