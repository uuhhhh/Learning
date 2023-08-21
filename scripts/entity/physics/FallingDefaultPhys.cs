using Godot;

namespace Learning.scripts.entity.physics; 

public partial class FallingDefaultPhys : Node, IDefaultPhys {
    [Export] public bool DoNotLink { get; set; }
    [Export] private Falling ToLink { get; set; }

    public void OnBecomeOnFloor(KinematicComp physics) {
        ToLink.IsFalling = false;
    }

    public void OnBecomeOffFloor(KinematicComp physics) {
        ToLink.IsFalling = true;
    }

    public void OnBecomeOnCeiling(KinematicComp physics) {
        ToLink.CeilingHitStop();
    }
}