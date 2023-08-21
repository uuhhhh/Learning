using Godot;

namespace Learning.scripts.entity.physics;

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
        Falling = GetNode<Falling>(nameof(Falling));
        LeftRight = GetNode<LeftRight>(nameof(LeftRight));
        Jumping = GetNode<Jumping>(nameof(Jumping));
        WallDragging = GetNode<WallDragging>(nameof(WallDragging));
        WallJumpInputTakeover = GetNode<Timer>(nameof(WallJumpInputTakeover));

        WallDraggingDefaultPhys = GetNode<WallDraggingDefaultPhys>(nameof(WallDraggingDefaultPhys));
        JumpingDefaultPhys = GetNode<JumpingDefaultPhys>(nameof(JumpingDefaultPhys));
        
        WallDragChecker = GetNode<KinematicComp>(nameof(WallDragChecker));
        _wallDragCheckerInitialPosition = WallDragChecker.Position;

        InitNumJumpsBehavior();
        InitWallJumpTakeoverBehavior();
        ModifyWallTouchingBehavior();
    }

    private void InitNumJumpsBehavior() {
        BecomeOnFloor += _ => Jumping.ResetNumJumps();
        WallDragging.StartedValidWallTouching += Jumping.ResetNumJumps;
    }

    private void InitWallJumpTakeoverBehavior() {
        Jumping.Jumped += from => {
            if (from == Location.WallNonGround) {
                WallJumpInputTakeover.Start();
            }
        };

        WallJumpInputTakeover.Timeout += () => LeftRight.IntendedSpeedScale = _playerLeftRightInput;
    }

    private void ModifyWallTouchingBehavior() {
        BecomeOnWall -= WallDraggingDefaultPhys.OnBecomeOnWall;
        BecomeOnWall += ValidWallTouchingCheck;
        WallDragChecker.BecomeOnWall += ValidWallTouchingCheck;
        
        BecomeOffFloor -= WallDraggingDefaultPhys.OnBecomeOffFloor;
        BecomeOffFloor += ValidWallTouchingCheck;
        WallDragChecker.BecomeOffFloor += ValidWallTouchingCheck;
        
        WallDragChecker.BecomeOffWall += _ => WallDragCheckerNotDraggingCheck();
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

    private void WallDragCheckerNotDraggingCheck() {
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
        ValidWallTouchingCheck(this);
        if (!(WallJumpInputTakeover.TimeLeft > 0)) {
            LeftRight.IntendedSpeedScale--;
        }
    }

    public void MoveRight() {
        _playerLeftRightInput++;
        ValidWallTouchingCheck(this);
        if (!(WallJumpInputTakeover.TimeLeft > 0)) {
            LeftRight.IntendedSpeedScale++;
        }
    }

    public void AttemptJump() {
        Jumping.AttemptJump();
    }

    public void JumpCancel() {
        Jumping.JumpCancel();
    }
}