using Godot;

namespace Learning.scripts.entity.physics; 

public interface IKinematicCompLinkable {
    [Export] public bool DoNotLink { get; protected set; }
    
    void Link(KinematicComp2 physics);
}