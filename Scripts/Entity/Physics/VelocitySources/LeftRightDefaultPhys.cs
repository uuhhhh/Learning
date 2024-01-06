using Godot;

namespace Learning.Scripts.Entity.Physics.VelocitySources;

/// <summary>
///     Default behavior for what a LeftRight will do, based on the actions of a KinematicComp.
/// </summary>
public partial class LeftRightDefaultPhys : DefaultPhys
{
    /// <summary>
    ///     The LeftRight to be controlled by the callback KinematicComp
    /// </summary>
    [Export]
    private LeftRight ToLink { get; set; }

    /// <summary>
    ///     When the given KinematicComp becomes on a floor,
    ///     set this LeftRight to act as if it's on the floor.
    /// </summary>
    internal override void OnBecomeOnFloor(KinematicComp physics)
    {
        ToLink.IsOnGround = true;
    }

    /// <summary>
    ///     When the given KinematicComp becomes not on a floor,
    ///     set this LeftRight to act as if it's in the air.
    /// </summary>
    internal override void OnBecomeOffFloor(KinematicComp physics)
    {
        ToLink.IsOnGround = false;
    }
}