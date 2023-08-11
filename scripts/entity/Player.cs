using Godot;

namespace Learning.scripts.entity;

public partial class Player : Node2D {
	private InputComp _input;
	private KinematicComp _physics;
	
	public override void _Ready() {
		_input = GetNode<InputComp>(nameof(InputComp));
		_physics = GetNode<KinematicComp>(nameof(KinematicComp));

		_input.LeftInputOn += () => { _physics.AddToInputSpeedScale(-1); };
		_input.LeftInputOff += () => { _physics.AddToInputSpeedScale(1); };
		_input.RightInputOn += () => { _physics.AddToInputSpeedScale(1); };
		_input.RightInputOff += () => { _physics.AddToInputSpeedScale(-1); };
		_input.JumpInputOn += _physics.AttemptJump;
		_input.JumpInputOff += _physics.JumpCancel;
	}

	public override void _PhysicsProcess(double delta) {
		_physics.SetParentPositionToOwn(this);
	}
}
