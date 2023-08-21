using Godot;
using Learning.scripts.entity.physics;

namespace Learning.scripts.entity; 

public partial class Player : Node2D {
    private InputComp _input;
    private PlayerKinematicComp _physics;

    public override void _Ready() {
        _input = GetNode<InputComp>(nameof(InputComp));
        _physics = GetNode<PlayerKinematicComp>(nameof(PlayerKinematicComp));

        _input.LeftInputOn += _physics.MoveLeft;
        _input.LeftInputOff += _physics.MoveRight;
        _input.RightInputOn += _physics.MoveRight;
        _input.RightInputOff += _physics.MoveLeft;
        _input.JumpInputOn += _physics.AttemptJump;
        _input.JumpInputOff += _physics.JumpCancel;

        ProcessPhysicsPriority = _physics.ProcessPhysicsPriority + 1;
    }

    public override void _PhysicsProcess(double delta) {
        _physics.SetParentPositionToOwn(this);
    }
}