using Godot;

namespace Learning.Scripts.Entity.Physics.VelocitySources;

public partial class LeftRightDefaultPhys : DefaultPhys
{
    [Export] private LeftRight ToLink { get; set; }

    internal override void OnBecomeOnFloor(KinematicComp physics)
    {
        ToLink.IsOnGround = true;
    }

    internal override void OnBecomeOffFloor(KinematicComp physics)
    {
        ToLink.IsOnGround = false;
    }
}