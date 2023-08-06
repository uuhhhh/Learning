using Godot;
using System;

public partial class Player : CharacterBody2D {
	private const float SPEED = 200f;
	private const float ACCELERATION = 750f;
	private const float FRICTION = 1000f;
	private const float JUMP_VELOCITY = -350f;
	
	private float _gravity = ProjectSettings.GetSetting("physics/2d/default_gravity").AsSingle();
	
	private InputComponent _input;

	private int _directionInputtingX;

	public override void _Ready() {
		_input = GetNode<InputComponent>("InputComponent");

		_input.LeftInputOn += () => { _directionInputtingX -= 1; };
		_input.LeftInputOff += () => { _directionInputtingX += 1; };
		_input.RightInputOn += () => { _directionInputtingX += 1; };
		_input.RightInputOff += () => { _directionInputtingX -= 1; };
		_input.JumpInputOn += Jump;
		_input.JumpInputOff += JumpCancel;
	}

	public override void _PhysicsProcess(double delta)
	{
		Vector2 velocity = Velocity;
		HandleInputVelocity(velocity, delta);
		HandleGravity(velocity, delta);
		Velocity = velocity;
		
		MoveAndSlide();
	}

	private void HandleInputVelocity(Vector2 velocity, double delta) {
		if (_directionInputtingX != 0) {
			velocity.X = Mathf.MoveToward(
				velocity.X,
				SPEED * _directionInputtingX,
				ACCELERATION * (float)delta);
		}
		else {
			velocity.X = Mathf.MoveToward(velocity.X, 0, FRICTION * (float)delta);
		}
	}

	private void HandleGravity(Vector2 velocity, double delta) {
		if (!IsOnFloor()) {
			velocity.Y += _gravity * (float)delta;
		}
	}

	private void Jump() {
		if (IsOnFloor()) {
			Vector2 velocity = Velocity;
			velocity.Y += JUMP_VELOCITY;
			Velocity = velocity;
		}
	}

	private void JumpCancel() {
		if (!IsOnFloor() && Velocity.Y < JUMP_VELOCITY / 2) {
			Vector2 velocity = Velocity;
			velocity.Y = JUMP_VELOCITY / 2;
			Velocity = velocity;
		}
	}
}
