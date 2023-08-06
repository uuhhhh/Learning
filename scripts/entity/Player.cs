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

	public override void _PhysicsProcess(double delta) {
		Velocity = HandleInputVelocity(Velocity, delta);
		Velocity = HandleGravity(Velocity, delta);
		
		MoveAndSlide();
	}

	private Vector2 HandleInputVelocity(Vector2 velocity, double delta) {
		Vector2 newVelocity = velocity;
		
		if (_directionInputtingX != 0) {
			newVelocity.X = Mathf.MoveToward(
				newVelocity.X,
				SPEED * _directionInputtingX,
				ACCELERATION * (float)delta);
		}
		else {
			newVelocity.X = Mathf.MoveToward(newVelocity.X, 0, FRICTION * (float)delta);
		}

		return newVelocity;
	}

	private Vector2 HandleGravity(Vector2 velocity, double delta) {
		Vector2 newVelocity = velocity;
		
		if (!IsOnFloor()) {
			newVelocity.Y += _gravity * (float)delta;
		}

		return newVelocity;
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
