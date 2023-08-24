using Godot;

namespace Learning.Scripts.Entity.Physics.VelocitySources; 

public partial class FallingDefaultPhys : Node, IDefaultPhys {
    [Export] public bool DoNotLink { get; set; }
    [Export] public bool DoNotCallExtraInit { get; set; }
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