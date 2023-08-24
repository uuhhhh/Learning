﻿using Godot;
using Learning.Scripts.Entity.Physics.Intermediate;
using Learning.Scripts.Entity.Physics.VelocitySources;

namespace Learning.Scripts.Entity.Physics;

public partial class PlayerVelocityAggregate : VelocityAggregate {
    public Falling Falling { get; private set; }
    public LeftRight LeftRight { get; private set; }
    public Jumping Jumping { get; private set; }
    public WallDragging WallDragging { get; private set; }
    public Timer WallJumpInputTakeover { get; private set; }
    
    private WallDraggingDefaultPhys WallDraggingDefaultPhys { get; set; }
    private JumpingDefaultPhys JumpingDefaultPhys { get; set; }
    private KinematicComp WallDragChecker { get; set; }

    private Vector2 _wallDragCheckerInitialPosition;
    
    private int _playerLeftRightInput;

    public override void _Ready() {
        base._Ready();
        SetChildren();

        InitNumJumpsBehavior();
        InitWallJumpInputTakeoverBehavior();
        InitWallTouchLeftRightStopBehavior();
        InitWallDragCheckerWallTouchingBehavior();
    }

    private void SetChildren() {
        Falling = GetNode<Falling>(nameof(Falling));
        LeftRight = GetNode<LeftRight>(nameof(LeftRight));
        Jumping = GetNode<Jumping>(nameof(Jumping));
        WallDragging = GetNode<WallDragging>(nameof(WallDragging));
        WallJumpInputTakeover = GetNode<Timer>(nameof(WallJumpInputTakeover));

        WallDraggingDefaultPhys = GetNode<WallDraggingDefaultPhys>(nameof(WallDraggingDefaultPhys));
        JumpingDefaultPhys = GetNode<JumpingDefaultPhys>(nameof(JumpingDefaultPhys));
        
        WallDragChecker = GetNode<KinematicComp>(nameof(WallDragChecker));
        _wallDragCheckerInitialPosition = WallDragChecker.Position;
    }

    private void InitNumJumpsBehavior() {
        BecomeOnFloor += _ => Jumping.ResetNumJumps();
        WallDragging.StartedValidWallTouching += Jumping.ResetNumJumps;
    }

    private void InitWallJumpInputTakeoverBehavior() {
        Jumping.Jumped += from => {
            if (from == Location.WallNonGround) {
                WallJumpInputTakeover.Start();
            }
        };
        WallJumpInputTakeover.Timeout += () => LeftRight.IntendedSpeedScale = _playerLeftRightInput;
    }

    private void InitWallTouchLeftRightStopBehavior() {
        // can't set to exactly 0 due to physics weirdness
        // (this body changing state to becoming off wall when Falling tweening to max velocity)
        float almostZero = 0.01f;
        WallDragging.StartedValidWallTouching += () => LeftRight.IntendedSpeedScale *= almostZero;
        
        BecomeOffWall += _ => UpdateLeftRightSpeed();
        BecomeOnFloor += _ => UpdateLeftRightSpeed();
    }

    private void InitWallDragCheckerWallTouchingBehavior() {
        BecomeOnWall -= WallDraggingDefaultPhys.OnBecomeOnWall;
        BecomeOnWall += ValidWallTouchingCheck;
        WallDragChecker.BecomeOnWall += ValidWallTouchingCheck;
        
        BecomeOffFloor -= WallDraggingDefaultPhys.OnBecomeOffFloor;
        BecomeOffFloor += ValidWallTouchingCheck;
        WallDragChecker.BecomeOffFloor += ValidWallTouchingCheck;
        
        WallDragChecker.BecomeOffWall += _ => NonWallDragCheckerPartStillDraggingCheck();
        WallDragChecker.BecomeOffWall += WallDraggingDefaultPhys.OnBecomeOffWall;
        
        BecomeOnWall -= JumpingDefaultPhys.OnBecomeOnWall;
        WallDragChecker.BecomeOnWall += JumpingDefaultPhys.OnBecomeOnWall;
    }

    private void ValidWallTouchingCheck(KinematicComp physics) {
        bool playerPressingAgainstWall = Mathf.Sign(GetWallNormal().X) == -Mathf.Sign(_playerLeftRightInput);
        bool noInputDragging = WallDragging.IsDragging && _playerLeftRightInput == 0;

        WallDragging.ValidWallTouching =
            WallDraggingDefaultPhys.IsOnValidWall(physics)
            && (playerPressingAgainstWall || noInputDragging)
            && WallDragChecker.IsOnWall();
    }

    private void NonWallDragCheckerPartStillDraggingCheck() {
        if (IsOnWall() && !WallDragChecker.IsOnWall() && Jumping.CurrentLocation == Location.WallNonGround) {
            Jumping.TransitionToAir(immediately: true);
        }
    }

    public override void _PhysicsProcess(double delta) {
        base._PhysicsProcess(delta);

        WallDragChecker.Position = _wallDragCheckerInitialPosition;
    }

    public void MoveLeft() {
        _playerLeftRightInput--;
        UpdateLeftRightSpeed();
    }

    public void MoveRight() {
        _playerLeftRightInput++;
        UpdateLeftRightSpeed();
    }

    private void UpdateLeftRightSpeed() {
        ValidWallTouchingCheck(this);
        
        JumpingDefaultPhys.DirectionGoing = _playerLeftRightInput;
        
        bool pressingOrStayingAgainstWall =
            WallDragging.ValidWallTouching
            && Mathf.Sign(GetWallNormal().X) != Mathf.Sign(_playerLeftRightInput);
        if (!pressingOrStayingAgainstWall && !(WallJumpInputTakeover.TimeLeft > 0)) {
            LeftRight.IntendedSpeedScale = _playerLeftRightInput;
        }
    }

    public void AttemptJump() {
        Jumping.AttemptJump();
    }

    public void JumpCancel() {
        Jumping.JumpCancel();
    }
}