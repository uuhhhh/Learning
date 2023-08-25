﻿using Godot;

namespace Learning.Scripts.Entity.Physics.Intermediate; 

public partial class JumpingDefaultPhys : DefaultPhys {
    [Export] private Jumping ToLink { get; set; }
    
    public float WallDirection {
        get => _wallDirection;
        set {
            _wallDirection = value;
            WallPressCheck();
        }
    }

    public float DirectionGoing {
        get => _directionGoing;
        set {
            _directionGoing = value;
            WallPressCheck();
        }
    }

    private float _wallDirection;
    private float _directionGoing;

    internal override void ExtraInit(KinematicComp physics) {
        ToLink.LeftRight.IntendedSpeedUpdate += speed => DirectionGoing = speed;
    }

    internal override void OnBecomeOnFloor(KinematicComp physics) {
        WallDirection = 0;
        if (ToLink.CurrentLocation != Location.Ground) {
            ToLink.TransitionToGround();
        }
    }

    internal override void OnBecomeOffFloor(KinematicComp physics) {
        if (physics.IsOnWall() && ToLink.CurrentLocation != Location.WallNonGround) {
            WallDirection = physics.GetWallNormal().X;
        } else if (ToLink.CurrentLocation != Location.Air) {
            ToLink.TransitionToAir();
        }
    }

    internal override void OnBecomeOnWall(KinematicComp physics) {
        if (!physics.IsOnFloor() && ToLink.CurrentLocation != Location.WallNonGround) {
            WallDirection = physics.GetWallNormal().X;
        }
    }

    internal override void OnBecomeOffWall(KinematicComp physics) {
        WallDirection = 0;
        if (!physics.IsOnFloor() && ToLink.CurrentLocation != Location.Air) {
            ToLink.TransitionToAir();
        }
    }

    internal void WallPressCheck() {
        int wallDirectionSign = Mathf.Sign(WallDirection);
        if (wallDirectionSign != 0 && (wallDirectionSign == -Mathf.Sign(DirectionGoing)
                                       || ToLink.CurrentLocation == Location.WallNonGround)) {
            ToLink.TransitionToWall(WallDirection);
        }
    }
}