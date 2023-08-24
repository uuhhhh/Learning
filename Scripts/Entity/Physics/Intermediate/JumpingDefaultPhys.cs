using Godot;

namespace Learning.Scripts.Entity.Physics.Intermediate; 

public partial class JumpingDefaultPhys : Node, IDefaultPhys {
    [Export] public bool DoNotLink { get; set; }
    [Export] public bool DoNotCallExtraInit { get; set; }
    [Export] private Jumping ToLink { get; set; }

    private float _wallDirection;
    private float _intendedLeftRightSpeed;

    public void ExtraInit(KinematicComp physics) {
        ToLink.LeftRight.IntendedSpeedUpdate += speed => {
            _intendedLeftRightSpeed = speed;
            WallPressCheck();
        };
    }

    public void OnBecomeOnFloor(KinematicComp physics) {
        _wallDirection = 0;
        if (ToLink.CurrentLocation != Location.Ground) {
            ToLink.TransitionToGround();
        }
    }

    public void OnBecomeOffFloor(KinematicComp physics) {
        if (physics.IsOnWall() && ToLink.CurrentLocation != Location.WallNonGround) {
            _wallDirection = physics.GetWallNormal().X;
            WallPressCheck();
        } else if (ToLink.CurrentLocation != Location.Air) {
            ToLink.TransitionToAir();
        }
    }

    public void OnBecomeOnWall(KinematicComp physics) {
        if (!physics.IsOnFloor() && ToLink.CurrentLocation != Location.WallNonGround) {
            _wallDirection = physics.GetWallNormal().X;
            WallPressCheck();
        }
    }

    public void OnBecomeOffWall(KinematicComp physics) {
        _wallDirection = 0;
        if (!physics.IsOnFloor() && ToLink.CurrentLocation != Location.Air) {
            ToLink.TransitionToAir();
        }
    }

    public void WallPressCheck() {
        int wallDirectionSign = Mathf.Sign(_wallDirection);
        if (wallDirectionSign != 0 && (wallDirectionSign == -Mathf.Sign(_intendedLeftRightSpeed)
                                       || ToLink.CurrentLocation == Location.Ground)) {
            ToLink.TransitionToWall(_wallDirection);
        }
    }
}