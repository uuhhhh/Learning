using Godot;

namespace Learning.Scripts.Entity.Physics.Intermediate; 

public partial class JumpingDefaultPhys : Node, IDefaultPhys {
    [Export] public bool DoNotLink { get; set; }
    [Export] private Jumping ToLink { get; set; }

    public void OnBecomeOnFloor(KinematicComp physics) {
        if (ToLink.CurrentLocation != Location.Ground) {
            ToLink.TransitionToGround();
        }
    }

    public void OnBecomeOffFloor(KinematicComp physics) {
        if (physics.IsOnWall() && ToLink.CurrentLocation != Location.WallNonGround) {
            ToLink.TransitionToWall(physics.GetWallNormal().X);
        } else if (ToLink.CurrentLocation != Location.Air) {
            ToLink.TransitionToAir();
        }
    }

    public void OnBecomeOnWall(KinematicComp physics) {
        if (!physics.IsOnFloor() && ToLink.CurrentLocation != Location.WallNonGround) {
            ToLink.TransitionToWall(physics.GetWallNormal().X);
        }
    }

    public void OnBecomeOffWall(KinematicComp physics) {
        if (!physics.IsOnFloor() && ToLink.CurrentLocation != Location.Air) {
            ToLink.TransitionToAir();
        }
    }
    
}