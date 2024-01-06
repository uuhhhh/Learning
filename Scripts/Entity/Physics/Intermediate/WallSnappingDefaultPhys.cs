using Godot;

namespace Learning.Scripts.Entity.Physics.Intermediate;

/// <summary>
///     Default behavior for what a WallSnapping will do, based on the actions of a KinematicComp
/// </summary>
public partial class WallSnappingDefaultPhys : DefaultPhys
{
    /// <summary>
    ///     The WallSnapping to be controlled by the callback KinematicComp.
    /// </summary>
    [Export]
    private WallSnapping ToLink { get; set; }

    /// <summary>
    ///     If the given KinematicComp falls off a floor, put the WallSnapping in the wall snap start
    ///     window state.
    /// </summary>
    internal override void OnBecomeOffFloor(KinematicComp physics)
    {
        if (physics.Velocity.Y >= 0) ToLink.InWallSnapStartWindow = true;
    }
}