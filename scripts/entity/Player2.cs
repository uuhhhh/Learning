using Godot;
using Learning.scripts.entity.physics;

namespace Learning.scripts.entity; 

public partial class Player2 : Node2D {
    private InputComp _input;
    private PlayerKinematicComp _physics;

    public override void _Ready() {
        _input = GetNode<InputComp>(nameof(InputComp));
        _physics = GetNode<PlayerKinematicComp>(nameof(PlayerKinematicComp));

        _input.LeftInputOn += () => _physics.LeftRight.IntendedSpeedScale -= 1;
        _input.LeftInputOff += () => _physics.LeftRight.IntendedSpeedScale += 1;
        _input.RightInputOn += () => _physics.LeftRight.IntendedSpeedScale += 1;
        _input.RightInputOff += () => _physics.LeftRight.IntendedSpeedScale -= 1;
        _input.JumpInputOn += () => _physics.Jumping.AttemptJump();
        _input.JumpInputOff += () => _physics.Jumping.JumpCancel();
    }

    public override void _PhysicsProcess(double delta) {
        _physics.SetParentPositionToOwn(this);
    }
}