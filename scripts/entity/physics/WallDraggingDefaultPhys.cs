using Godot;

namespace Learning.scripts.entity.physics; 

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
        if (ToLink.IsOnValidWall(physics)) {
            ToLink.ValidWallTouching = true;
        }
    }

    public void OnBecomeOffWall(KinematicComp physics) {
        ToLink.ValidWallTouching = false;
        ToLink.IsDragging = false;
    }
}