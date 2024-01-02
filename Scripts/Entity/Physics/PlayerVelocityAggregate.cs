using Godot;
using Learning.Scripts.Entity.Physics.Intermediate;
using Learning.Scripts.Entity.Physics.VelocitySources;

namespace Learning.Scripts.Entity.Physics;

public partial class PlayerVelocityAggregate : VelocityAggregate {
    [Export] private KinematicComp PhysicsInteractions { get; set; }
    
    public Falling Falling { get; private set; }
    public LeftRight LeftRight { get; private set; }
    
    public Jumping Jumping { get; private set; }
    public WallDragging WallDragging { get; private set; }
    public WallSnapping WallSnapping { get; private set; }
    
    private JumpingDefaultPhys JumpingDefaultPhys { get; set; }
    private WallDraggingDefaultPhys WallDraggingDefaultPhys { get; set; }
    
    private Timer WallJumpInputTakeover { get; set; }

    public bool CanDoWallBehavior {
        get => _canDoWallBehavior;
        set {
            _canDoWallBehavior = value;

            ValidWallTouchingCheck(PhysicsInteractions);
            
            if (!_canDoWallBehavior && Jumping.CurrentLocationAfterTransition == Location.WallNonGround) {
                Jumping.TransitionToAir(immediately: true);
            }
        }
    }

    private Vector2 _wallDragCheckerInitialPosition;
    
    private int _playerLeftRightInput;

    private bool _canDoWallBehavior;

    public override void _Ready() {
        base._Ready();
        SetChildren();

        InitNumJumpsBehavior();
        InitWallJumpInputTakeoverBehavior();
        InitWallTouchLeftRightStopBehavior();
        InitWallSnappingBehavior();
        ModifyWallTouchingBehavior();
    }

    private void SetChildren() {
        Falling = GetNode<Falling>(nameof(Falling));
        LeftRight = GetNode<LeftRight>(nameof(LeftRight));
        
        Jumping = GetNode<Jumping>(nameof(Jumping));
        WallDragging = GetNode<WallDragging>(nameof(WallDragging));
        WallSnapping = GetNode<WallSnapping>(nameof(WallSnapping));
        
        JumpingDefaultPhys = GetNode<JumpingDefaultPhys>(nameof(JumpingDefaultPhys));
        WallDraggingDefaultPhys = GetNode<WallDraggingDefaultPhys>(nameof(WallDraggingDefaultPhys));
        
        WallJumpInputTakeover = GetNode<Timer>(nameof(WallJumpInputTakeover));
    }

    private void InitNumJumpsBehavior() {
        PhysicsInteractions.BecomeOnFloor += _ => Jumping.ResetNumJumps();
        WallDragging.StartedValidWallTouching += Jumping.ResetNumJumps;
    }

    private void InitWallJumpInputTakeoverBehavior() {
        Jumping.Jumped += from => {
            if (from == Location.WallNonGround) {
                WallJumpInputTakeover.Start();
            }
        };
        WallJumpInputTakeover.Timeout += UpdateLeftRightSpeed;
    }

    private void InitWallTouchLeftRightStopBehavior() {
        // can't set to exactly 0 due to physics weirdness
        // (this body changing state to becoming off wall when Falling tweening from above max velocity to max velocity)
        WallDragging.StartedValidWallTouching += () =>
            LeftRight.IntendedSpeed = Mathf.Sign(LeftRight.IntendedSpeed);
        
        PhysicsInteractions.BecomeOffWall += _ => UpdateLeftRightSpeedIfAble();
        PhysicsInteractions.BecomeOnFloor += _ => UpdateLeftRightSpeedIfAble();
    }
    
    private void InitWallSnappingBehavior() {
        WallSnapping.WallSnapStopped += () => {
            if (!PhysicsInteractions.IsOnWall()) {
                UpdateLeftRightSpeed();
            }
        };
        WallDragging.StartedValidWallTouching += () => WallSnapping.IsWallSnapping = false;
        PhysicsInteractions.BecomeOffWall += _ => {
            if (WallSnapping.IsWallSnapping) {
                WallSnapping.IsWallSnapping = false;
                UpdateLeftRightSpeed();
            }
        };
    }

    private void ModifyWallTouchingBehavior() {
        PhysicsInteractions.BecomeOnWall -= WallDraggingDefaultPhys.OnBecomeOnWall;
        PhysicsInteractions.BecomeOnWall -= JumpingDefaultPhys.OnBecomeOnWall;
        PhysicsInteractions.BecomeOnWall += ValidWallTouchingCheck;
        
        PhysicsInteractions.BecomeOffFloor -= WallDraggingDefaultPhys.OnBecomeOffFloor;
        PhysicsInteractions.BecomeOffFloor += ValidWallTouchingCheck;
    }

    private void ValidWallTouchingCheck(KinematicComp physics) {
        bool playerPressingAgainstWall = Mathf.Sign(physics.GetWallNormal().X) == -Mathf.Sign(_playerLeftRightInput);
        bool noInputDragging = WallDragging.IsDragging && _playerLeftRightInput == 0;
        
        bool isValidWallPressing = playerPressingAgainstWall || noInputDragging || WallSnapping.IsWallSnapping;

        WallDragging.ValidWallTouching = WallDraggingDefaultPhys.IsOnValidWall(physics)
                                         && isValidWallPressing
                                         && CanDoWallBehavior;

        if (physics.IsOnWall() && CanDoWallBehavior) {
            JumpingDefaultPhys.OnBecomeOnWall(physics);
        }
    }

    public void MoveLeft() {
        _playerLeftRightInput--;
        UpdateLeftRightSpeedIfAble();
    }

    public void MoveRight() {
        _playerLeftRightInput++;
        UpdateLeftRightSpeedIfAble();
    }

    private void UpdateLeftRightSpeedIfAble() {
        ValidWallTouchingCheck(PhysicsInteractions);
        
        WallSnapOppositeInputCheck();

        if (!WallSnapping.IsWallSnapping) {
            JumpingDefaultPhys.DirectionGoing = _playerLeftRightInput;
        }
        
        bool pressingOrStayingAgainstWall =
            WallDragging.ValidWallTouching
            && Mathf.Sign(PhysicsInteractions.GetWallNormal().X) != Mathf.Sign(_playerLeftRightInput);
        if (!pressingOrStayingAgainstWall
            && !(WallJumpInputTakeover.TimeLeft > 0)
            && !WallSnapping.IsWallSnapping) {
            UpdateLeftRightSpeed();
        }
    }

    private void WallSnapOppositeInputCheck() {
        if (WallSnapping.IsWallSnapping
            && (_playerLeftRightInput == 0
                || Mathf.Sign(_playerLeftRightInput) == -Mathf.Sign(LeftRight.IntendedSpeedScale))) {
            WallSnapping.IsWallSnapping = false;
        }
    }

    private void UpdateLeftRightSpeed() {
        LeftRight.IntendedSpeedScale = _playerLeftRightInput;
    }

    public void AttemptJump() {
        Jumping.AttemptJump();
    }

    public void JumpCancel() {
        Jumping.JumpCancel();
    }
}