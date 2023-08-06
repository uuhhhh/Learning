using Godot;
using System;

public partial class Player : CharacterBody2D {
	private const float SPEED = 200f;
	private const float JUMP_VELOCITY = -350f;
	private const float TIME_TO_FULL_SPEED_SECONDS = .25f;
	private const float TIME_TO_STOP_SECONDS = .125f;
	
	private float _gravity = ProjectSettings.GetSetting("physics/2d/default_gravity").AsSingle();
	
	private InputComponent _input;

	private int _directionInputtingX;

	private float _inputVelocityX;
	private float _finalInputVelocityX;
	private Tween _inputVelocityTween;

	public override void _Ready() {
		_input = GetNode<InputComponent>("InputComponent");

		_input.LeftInputOn += () => { MoveX(-1); };
		_input.LeftInputOff += () => { MoveX(1); };
		_input.RightInputOn += () => { MoveX(1); };
		_input.RightInputOff += () => { MoveX(-1); };
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
		
		newVelocity.X = _inputVelocityX;

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

	private void MoveX(float magnitude) {
		if (_inputVelocityTween != null && _inputVelocityTween.IsValid()) {
			_inputVelocityTween.Kill();
		}

		_finalInputVelocityX = Mathf.Round(_finalInputVelocityX + SPEED * magnitude);

		_inputVelocityTween = CreateTween();
		_inputVelocityTween.Parallel().TweenProperty(
			this,
			"_inputVelocityX",
			_finalInputVelocityX,
			_finalInputVelocityX == 0 ? TIME_TO_STOP_SECONDS : TIME_TO_FULL_SPEED_SECONDS);
	}
}
