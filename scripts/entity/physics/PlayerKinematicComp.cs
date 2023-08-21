using Godot;

namespace Learning.scripts.entity.physics;

public partial class PlayerKinematicComp : KinematicComp {
    public Falling Falling { get; private set; }
    public LeftRight LeftRight { get; private set; }
    public Jumping Jumping { get; private set; }
    public WallDragging WallDragging { get; private set; }
    public Timer WallJumpInputTakeover { get; private set; }
    
    private WallDraggingDefaultPhys WallDraggingDefaultPhys { get; set; }

    private int _playerLeftRightInput;

    public override void _Ready() {
        base._Ready();
        Falling = GetNode<Falling>(nameof(Falling));
        LeftRight = GetNode<LeftRight>(nameof(LeftRight));
        Jumping = GetNode<Jumping>(nameof(Jumping));
        WallDragging = GetNode<WallDragging>(nameof(WallDragging));
        WallJumpInputTakeover = GetNode<Timer>(nameof(WallJumpInputTakeover));

        WallDraggingDefaultPhys = GetNode<WallDraggingDefaultPhys>(nameof(WallDraggingDefaultPhys));

        InitNumJumpsBehavior();
        InitWallJumpTakeoverBehavior();
        ModifyWallTouchingBehavior();
    }

    private void InitNumJumpsBehavior() {
        BecomeOnFloor += _ => Jumping.ResetNumJumps();
        BecomeOnWall += _ => Jumping.ResetNumJumps();
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
        BecomeOffFloor -= WallDraggingDefaultPhys.OnBecomeOffFloor;
        BecomeOffFloor += ValidWallTouchingCheck;
    }

    private void ValidWallTouchingCheck(KinematicComp physics) {
        bool playerPressingAgainstWall = Mathf.Sign(GetWallNormal().X) == -Mathf.Sign(_playerLeftRightInput);
        bool noInputDragging = WallDragging.IsDragging && _playerLeftRightInput == 0;

        WallDragging.ValidWallTouching =
            WallDragging.IsOnValidWall(physics)
            && (playerPressingAgainstWall || noInputDragging);
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