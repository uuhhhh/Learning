using Godot;
using System;

public partial class CharacterComponent : CharacterBody2D {
	[Export] private float _speed = 200f;
	[Export] private float _jumpVelocity = -350f;
	[Export] private float _timeToFullSpeedSeconds = .25f;
	[Export] private float _timeToStopSeconds = .125f;
	
	private float _gravity = ProjectSettings.GetSetting("physics/2d/default_gravity").AsSingle();

	private float _inputVelocityX;
	private float _finalInputVelocityX;
	private Tween _inputVelocityTween;

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

	public void Jump() {
		if (IsOnFloor()) {
			Vector2 velocity = Velocity;
			velocity.Y += _jumpVelocity;
			Velocity = velocity;
		}
	}

	public void JumpCancel() {
		if (!IsOnFloor() && Velocity.Y < _jumpVelocity / 2) {
			Vector2 velocity = Velocity;
			velocity.Y = _jumpVelocity / 2;
			Velocity = velocity;
		}
	}

	public void MoveX(float magnitude) {
		if (_inputVelocityTween != null && _inputVelocityTween.IsValid()) {
			_inputVelocityTween.Kill();
		}

		_finalInputVelocityX = Mathf.Round(_finalInputVelocityX + _speed * magnitude);

		_inputVelocityTween = CreateTween();
		_inputVelocityTween.Parallel().TweenProperty(
			this,
			"_inputVelocityX",
			_finalInputVelocityX,
			_finalInputVelocityX == 0 ? _timeToStopSeconds : _timeToFullSpeedSeconds);
	}

	public void SetParentPositionToOwn(Node2D parent) {
		parent.Position = GlobalPosition;
		Position = Vector2.Zero;
	}
}
