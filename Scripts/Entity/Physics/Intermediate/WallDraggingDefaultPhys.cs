using Godot;

namespace Learning.Scripts.Entity.Physics.Intermediate; 

public partial class WallDraggingDefaultPhys : Node, IDefaultPhys {
    [Export] public bool DoNotLink { get; set; }
    [Export] private WallDragging ToLink { get; set; }

    public void OnBecomeOnFloor(KinematicComp physics) {
        OnBecomeOffWall(physics);
    }

    public void OnBecomeOffFloor(KinematicComp physics) {
        OnBecomeOnWall(physics);
    }

    public void OnBecomeOnWall(KinematicComp physics) {
        if (IsOnValidWall(physics)) {
            ToLink.ValidWallTouching = true;
        }
    }

    public void OnBecomeOffWall(KinematicComp physics) {
        ToLink.ValidWallTouching = false;
        ToLink.IsDragging = false;
    }

    public static bool IsOnValidWall(CharacterBody2D physics) {
        return !physics.IsOnFloor() && physics.IsOnWall() && physics.GetWallNormal().Y == 0;
    }
}