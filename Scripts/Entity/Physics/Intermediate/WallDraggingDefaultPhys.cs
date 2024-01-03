using Godot;

namespace Learning.Scripts.Entity.Physics.Intermediate;

public partial class WallDraggingDefaultPhys : DefaultPhys
{
    [Export] private WallDragging ToLink { get; set; }

    internal override void OnBecomeOnFloor(KinematicComp physics)
    {
        OnBecomeOffWall(physics);
    }

    internal override void OnBecomeOffFloor(KinematicComp physics)
    {
        OnBecomeOnWall(physics);
    }

    internal override void OnBecomeOnWall(KinematicComp physics)
    {
        if (IsOnValidWall(physics)) ToLink.ValidWallTouching = true;
    }

    internal override void OnBecomeOffWall(KinematicComp physics)
    {
        ToLink.ValidWallTouching = false;
    }

    internal static bool IsOnValidWall(CharacterBody2D physics)
    {
        return !physics.IsOnFloor() && physics.IsOnWall() && physics.GetWallNormal().Y == 0;
    }
}