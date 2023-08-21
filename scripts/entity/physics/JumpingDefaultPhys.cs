using Godot;

namespace Learning.scripts.entity.physics; 

public partial class JumpingDefaultPhys : Node, IDefaultPhys {
    [Export] public bool DoNotLink { get; set; }
    [Export] private Jumping ToLink { get; set; }

    public void OnBecomeOnFloor(KinematicComp physics) {
        ToLink.TransitionToGround();
    }

    public void OnBecomeOffFloor(KinematicComp physics) {
        if (physics.IsOnWall()) {
            ToLink.TransitionToWall(physics.GetWallNormal().X);
        } else {
            ToLink.TransitionToAir();
        }
    }

    public void OnBecomeOnWall(KinematicComp physics) {
        if (!physics.IsOnFloor()) {
            ToLink.TransitionToWall(physics.GetWallNormal().X);
        }
    }

    public void OnBecomeOffWall(KinematicComp physics) {
        if (!physics.IsOnFloor()) {
            ToLink.TransitionToAir();
        }
    }
    
}