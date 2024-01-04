using Godot;

namespace Learning.Scripts.Entity.Physics.VelocitySources;

/// <summary>
/// Default behavior for what a Falling will do, based on the actions of a KinematicComp.
/// </summary>
public partial class FallingDefaultPhys : DefaultPhys
{
    [Export] private Falling ToLink { get; set; }

    /// <summary>
    /// When the given KinematicComp becomes on a floor, the Falling will stop falling.
    /// </summary>
    internal override void OnBecomeOnFloor(KinematicComp physics)
    {
        ToLink.IsFalling = false;
    }

    /// <summary>
    /// When the given KinematicComp becomes not on a floor, the Falling will start falling.
    /// </summary>
    internal override void OnBecomeOffFloor(KinematicComp physics)
    {
        ToLink.IsFalling = true;
    }

    /// <summary>
    /// When the given KinematicComp becomes on a ceiling,
    /// the Falling will act as if it hit a ceiling.
    /// </summary>
    internal override void OnBecomeOnCeiling(KinematicComp physics)
    {
        ToLink.CeilingHitStop();
    }
}