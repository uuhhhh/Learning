using Godot;

namespace Learning.scripts.entity.physics; 

public interface IKinematicCompLinkable {
    [Export] bool DoNotLink { get; protected set; }

    void Link(KinematicComp physics) {
        physics.BecomeOnFloor += DefaultOnBecomeOnFloor;
        physics.BecomeOffFloor += DefaultOnBecomeOffFloor;
        physics.BecomeOnCeiling += DefaultOnBecomeOnCeiling;
        physics.BecomeOffCeiling += DefaultOnBecomeOffCeiling;
        physics.BecomeOnWall += DefaultOnBecomeOnWall;
        physics.BecomeOffWall += DefaultOnBecomeOffWall;
        physics.DirectionChangeX += DefaultOnDirectionChangeX;
        physics.DirectionChangeY += DefaultOnDirectionChangeY;
    }

    void DefaultOnBecomeOnFloor(KinematicComp physics) {}
    
    void DefaultOnBecomeOffFloor(KinematicComp physics) {}
    
    void DefaultOnBecomeOnCeiling(KinematicComp physics) {}
    
    void DefaultOnBecomeOffCeiling(KinematicComp physics) {}
    
    void DefaultOnBecomeOnWall(KinematicComp physics) {}
    
    void DefaultOnBecomeOffWall(KinematicComp physics) {}
    
    void DefaultOnDirectionChangeX(KinematicComp physics, float newDirection) {}
    
    void DefaultOnDirectionChangeY(KinematicComp physics, float newDirection) {}
}