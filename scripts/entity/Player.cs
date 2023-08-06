using Godot;

public partial class Player : Node2D {
	private InputComponent _input;
	private CharacterComponent _physics;
	
	public override void _Ready() {
		_input = GetNode<InputComponent>("InputComponent");
		_physics = GetNode <CharacterComponent>("CharacterComponent");

		_input.LeftInputOn += () => { _physics.MoveX(-1); };
		_input.LeftInputOff += () => { _physics.MoveX(1); };
		_input.RightInputOn += () => { _physics.MoveX(1); };
		_input.RightInputOff += () => { _physics.MoveX(-1); };
		_input.JumpInputOn += _physics.Jump;
		_input.JumpInputOff += _physics.JumpCancel;
	}

	public override void _PhysicsProcess(double delta) {
		_physics.SetParentPositionToOwn(this);
	}
}
