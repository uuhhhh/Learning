using Godot;

namespace Learning.scripts.entity.physics; 

public interface IDefaultPhys {
    [Export] bool DoNotLink { get; protected set; }

    void Link(KinematicComp physics) {
        physics.BecomeOnFloor += OnBecomeOnFloor;
        physics.BecomeOffFloor += OnBecomeOffFloor;
        physics.BecomeOnCeiling += OnBecomeOnCeiling;
        physics.BecomeOffCeiling += OnBecomeOffCeiling;
        physics.BecomeOnWall += OnBecomeOnWall;
        physics.BecomeOffWall += OnBecomeOffWall;
        physics.DirectionChangeX += OnDirectionChangeX;
        physics.DirectionChangeY += OnDirectionChangeY;
    }

    void OnBecomeOnFloor(KinematicComp physics) {}
    
    void OnBecomeOffFloor(KinematicComp physics) {}
    
    void OnBecomeOnCeiling(KinematicComp physics) {}
    
    void OnBecomeOffCeiling(KinematicComp physics) {}
    
    void OnBecomeOnWall(KinematicComp physics) {}
    
    void OnBecomeOffWall(KinematicComp physics) {}
    
    void OnDirectionChangeX(KinematicComp physics, float newDirection) {}
    
    void OnDirectionChangeY(KinematicComp physics, float newDirection) {}
}