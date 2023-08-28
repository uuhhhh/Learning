using Godot;
using Learning.Scripts.Entity.Physics.Intermediate;
using Learning.Scripts.Entity.Physics.VelocitySources;

namespace Learning.Scripts.Entity.Physics;

public partial class PlayerVelocityAggregate : VelocityAggregate {
    public Falling Falling { get; private set; }
    public LeftRight LeftRight { get; private set; }
    public Jumping Jumping { get; private set; }
    public WallDragging WallDragging { get; private set; }
    
    private Timer WallJumpInputTakeover { get; set; }
    private WallDraggingDefaultPhys WallDraggingDefaultPhys { get; set; }
    private JumpingDefaultPhys JumpingDefaultPhys { get; set; }

    public bool CanDoWallBehavior {
        get => _canDoWallBehavior;
        set {
            _canDoWallBehavior = value;

            ValidWallTouchingCheck(this);
            
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
        ModifyWallTouchingBehavior();
    }

    private void SetChildren() {
        Falling = GetNode<Falling>(nameof(Falling));
        LeftRight = GetNode<LeftRight>(nameof(LeftRight));
        Jumping = GetNode<Jumping>(nameof(Jumping));
        WallDragging = GetNode<WallDragging>(nameof(WallDragging));
        WallJumpInputTakeover = GetNode<Timer>(nameof(WallJumpInputTakeover));

        WallDraggingDefaultPhys = GetNode<WallDraggingDefaultPhys>(nameof(WallDraggingDefaultPhys));
        JumpingDefaultPhys = GetNode<JumpingDefaultPhys>(nameof(JumpingDefaultPhys));
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
        // (this body changing state to becoming off wall when Falling tweening from above max velocity to max velocity)
        WallDragging.StartedValidWallTouching += () =>
            LeftRight.IntendedSpeed = Mathf.Sign(LeftRight.IntendedSpeed);
        
        BecomeOffWall += _ => UpdateLeftRightSpeed();
        BecomeOnFloor += _ => UpdateLeftRightSpeed();
    }

    private void ModifyWallTouchingBehavior() {
        BecomeOnWall -= WallDraggingDefaultPhys.OnBecomeOnWall;
        BecomeOnWall -= JumpingDefaultPhys.OnBecomeOnWall;
        BecomeOnWall += ValidWallTouchingCheck;
        
        BecomeOffFloor -= WallDraggingDefaultPhys.OnBecomeOffFloor;
        BecomeOffFloor += ValidWallTouchingCheck;
    }

    private void ValidWallTouchingCheck(KinematicComp physics) {
        bool playerPressingAgainstWall = Mathf.Sign(physics.GetWallNormal().X) == -Mathf.Sign(_playerLeftRightInput);
        bool noInputDragging = WallDragging.IsDragging && _playerLeftRightInput == 0;

        WallDragging.ValidWallTouching =
            WallDraggingDefaultPhys.IsOnValidWall(physics)
            && (playerPressingAgainstWall || noInputDragging)
            && CanDoWallBehavior;

        if (physics.IsOnWall() && CanDoWallBehavior) {
            JumpingDefaultPhys.OnBecomeOnWall(physics);
        }
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