using Godot;

namespace Learning.Scripts.Entity.Physics; 

public abstract partial class DefaultPhys : Node {
    [Export] internal bool DoNotLink { get; set; }
    [Export] internal bool DoNotCallExtraInit { get; set; }

    internal void Link(KinematicComp physics) {
        physics.BecomeOnFloor += OnBecomeOnFloor;
        physics.BecomeOffFloor += OnBecomeOffFloor;
        physics.BecomeOnCeiling += OnBecomeOnCeiling;
        physics.BecomeOffCeiling += OnBecomeOffCeiling;
        physics.BecomeOnWall += OnBecomeOnWall;
        physics.BecomeOffWall += OnBecomeOffWall;
        physics.DirectionChangeX += OnDirectionChangeX;
        physics.DirectionChangeY += OnDirectionChangeY;
    }

    internal virtual void ExtraInit(KinematicComp physics) {}

    internal virtual void OnBecomeOnFloor(KinematicComp physics) {}
    
    internal virtual void OnBecomeOffFloor(KinematicComp physics) {}
    
    internal virtual void OnBecomeOnCeiling(KinematicComp physics) {}
    
    internal virtual void OnBecomeOffCeiling(KinematicComp physics) {}
    
    internal virtual void OnBecomeOnWall(KinematicComp physics) {}
    
    internal virtual void OnBecomeOffWall(KinematicComp physics) {}
    
    internal virtual void OnDirectionChangeX(KinematicComp physics, float newDirection) {}
    
    internal virtual void OnDirectionChangeY(KinematicComp physics, float newDirection) {}
}