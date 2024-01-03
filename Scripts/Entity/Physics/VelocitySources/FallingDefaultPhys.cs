using Godot;

namespace Learning.Scripts.Entity.Physics.VelocitySources;

public partial class FallingDefaultPhys : DefaultPhys
{
    [Export] private Falling ToLink { get; set; }

    internal override void OnBecomeOnFloor(KinematicComp physics)
    {
        ToLink.IsFalling = false;
    }

    internal override void OnBecomeOffFloor(KinematicComp physics)
    {
        ToLink.IsFalling = true;
    }

    internal override void OnBecomeOnCeiling(KinematicComp physics)
    {
        ToLink.CeilingHitStop();
    }
}