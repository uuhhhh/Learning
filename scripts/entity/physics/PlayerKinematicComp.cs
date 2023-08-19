using Godot;

namespace Learning.scripts.entity.physics;

public partial class PlayerKinematicComp : KinematicComp2 {
    public Falling Falling { get; private set; }
    public LeftRight LeftRight { get; private set; }
    public Jumping Jumping { get; private set; }
    public Timer WallJumpInputTakeover { get; private set; }

    private int _playerLeftRightInput;

    public override void _Ready() {
        base._Ready();
        Falling = GetNode<Falling>(nameof(Falling));
        LeftRight = GetNode<LeftRight>(nameof(LeftRight));
        Jumping = GetNode<Jumping>(nameof(Jumping));
        WallJumpInputTakeover = GetNode<Timer>(nameof(WallJumpInputTakeover));

        BecomeOnFloor += _ => { Jumping.ResetNumJumps(); };
        BecomeOnWall += _ => { Jumping.ResetNumJumps(); };

        Jumping.Jumped += from => {
            if (from == Location.WallNonGround) {
                WallJumpInputTakeover.Start();
            }
        };

        WallJumpInputTakeover.Timeout += () => LeftRight.IntendedSpeedScale = _playerLeftRightInput;
    }

    public void MoveLeft() {
        _playerLeftRightInput--;
        if (!(WallJumpInputTakeover.TimeLeft > 0)) {
            LeftRight.IntendedSpeedScale--;
        }
    }

    public void MoveRight() {
        _playerLeftRightInput++;
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