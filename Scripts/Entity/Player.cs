using Godot;
using Learning.Scripts.Entity.Physics;

namespace Learning.Scripts.Entity; 

public partial class Player : Node2D {
    public InputComp Input { get; private set; }
    public PlayerVelocityAggregate Physics { get; private set; }
    public FloorDetector FloorDetector { get; private set; }
    public WallDetector WallDetector { get; private set; }

    public override void _Ready() {
        SetChildren();

        ConfigureInput();

        WallDetector.ZeroToOneEnvObjects += () => Physics.CanDoWallBehavior = true;
        WallDetector.ZeroEnvObjects += () => Physics.CanDoWallBehavior = false;
        
        ProcessPhysicsPriority = Physics.ProcessPhysicsPriority + 1;
    }

    private void SetChildren() {
        Input = GetNode<InputComp>(nameof(InputComp));
        Physics = GetNode<PlayerVelocityAggregate>(nameof(PlayerVelocityAggregate));
        FloorDetector = GetNode<FloorDetector>(nameof(FloorDetector));
        WallDetector = GetNode<WallDetector>(nameof(WallDetector));
    }

    private void ConfigureInput() {
        Input.LeftInputOn += Physics.MoveLeft;
        Input.LeftInputOff += Physics.MoveRight;
        Input.RightInputOn += Physics.MoveRight;
        Input.RightInputOff += Physics.MoveLeft;
        Input.JumpInputOn += Physics.AttemptJump;
        Input.JumpInputOff += Physics.JumpCancel;
    }

    public override void _PhysicsProcess(double delta) {
        Physics.SetParentPositionToOwn(this);
    }
}