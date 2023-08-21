using Godot;

namespace Learning.scripts.entity.physics; 

public interface IKinematicCompLinkable {
    [Export] bool DoNotLink { get; protected set; }

    void Link(KinematicComp2 physics) {
        physics.BecomeOnFloor += DefaultOnBecomeOnFloor;
        physics.BecomeOffFloor += DefaultOnBecomeOffFloor;
        physics.BecomeOnCeiling += DefaultOnBecomeOnCeiling;
        physics.BecomeOffCeiling += DefaultOnBecomeOffCeiling;
        physics.BecomeOnWall += DefaultOnBecomeOnWall;
        physics.BecomeOffWall += DefaultOnBecomeOffWall;
        physics.DirectionChangeX += DefaultOnDirectionChangeX;
        physics.DirectionChangeY += DefaultOnDirectionChangeY;
    }

    void DefaultOnBecomeOnFloor(KinematicComp2 physics) {}
    
    void DefaultOnBecomeOffFloor(KinematicComp2 physics) {}
    
    void DefaultOnBecomeOnCeiling(KinematicComp2 physics) {}
    
    void DefaultOnBecomeOffCeiling(KinematicComp2 physics) {}
    
    void DefaultOnBecomeOnWall(KinematicComp2 physics) {}
    
    void DefaultOnBecomeOffWall(KinematicComp2 physics) {}
    
    void DefaultOnDirectionChangeX(KinematicComp2 physics, float newDirection) {}
    
    void DefaultOnDirectionChangeY(KinematicComp2 physics, float newDirection) {}
}