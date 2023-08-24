using Godot;

namespace Learning.Scripts.Entity.Physics.VelocitySources;

public partial class LeftRightDefaultPhys : Node, IDefaultPhys {
    [Export] public bool DoNotLink { get; set; }
    [Export] public bool DoNotCallExtraInit { get; set; }
    [Export] private LeftRight ToLink { get; set; }

    public void OnBecomeOnFloor(KinematicComp physics) {
        ToLink.IsOnGround = true;
    }

    public void OnBecomeOffFloor(KinematicComp physics) {
        ToLink.IsOnGround = false;
    }
}