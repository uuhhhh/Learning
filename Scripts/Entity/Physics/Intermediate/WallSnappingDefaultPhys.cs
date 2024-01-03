using Godot;

namespace Learning.Scripts.Entity.Physics.Intermediate;

public partial class WallSnappingDefaultPhys : DefaultPhys
{
    [Export] private WallSnapping ToLink { get; set; }

    internal override void OnBecomeOffFloor(KinematicComp physics)
    {
        if (physics.Velocity.Y >= 0) ToLink.InWallSnapStartWindow = true;
    }
}