using Godot;
using Learning.Scripts.Entity.Physics;

namespace Learning.Scripts.Entity; 

public partial class Player : KinematicComp {
    public InputComp Input { get; private set; }
    public PlayerVelocityAggregate PlayerController { get; private set; }
    public FloorDetector FloorDetector { get; private set; }
    public WallDetector WallDetector { get; private set; }

    public override void _Ready() {
        base._Ready();
        
        SetChildren();

        ConfigureInput();

        WallDetector.ZeroToOneEnvObjects += () => PlayerController.CanDoWallBehavior = true;
        WallDetector.ZeroEnvObjects += () => PlayerController.CanDoWallBehavior = false;
    }

    private void SetChildren() {
        Input = GetNode<InputComp>(nameof(InputComp));
        PlayerController = GetNode<PlayerVelocityAggregate>(nameof(PlayerVelocityAggregate));
        FloorDetector = GetNode<FloorDetector>(nameof(FloorDetector));
        WallDetector = GetNode<WallDetector>(nameof(WallDetector));
    }

    private void ConfigureInput() {
        Input.LeftInputOn += PlayerController.MoveLeft;
        Input.LeftInputOff += PlayerController.MoveRight;
        Input.RightInputOn += PlayerController.MoveRight;
        Input.RightInputOff += PlayerController.MoveLeft;
        Input.JumpInputOn += PlayerController.AttemptJump;
        Input.JumpInputOff += PlayerController.JumpCancel;
    }

    public override void _PhysicsProcess(double delta) {
        Velocity = PlayerController.Velocity;
		
        MoveAndSlideWithStatusChanges();
    }
}