using Learning.Scripts.Effects;
using Learning.Scripts.Entity.Physics;
using Learning.Scripts.Entity.Physics.Intermediate;

namespace Learning.Scripts.Entity; 

public partial class Player : KinematicComp {
    public InputComp Input { get; private set; }
    public PlayerVelocityAggregate PlayerController { get; private set; }
    public FloorDetector FloorDetector { get; private set; }
    public WallDetector WallDetector { get; private set; }
    public ImpactParticles GroundJumpParticles { get; private set; }
    public ImpactParticles AirJumpParticles { get; private set; }
    public ImpactParticles LeftWallJumpParticles { get; private set; }
    public ImpactParticles RightWallJumpParticles { get; private set; }

    public override void _Ready() {
        base._Ready();
        
        SetChildren();

        ConfigureInput();
        
        InitParticlesBehavior();

        WallDetector.ZeroToOneEnvObjects += () => PlayerController.CanDoWallBehavior = true;
        WallDetector.ZeroEnvObjects += () => PlayerController.CanDoWallBehavior = false;
    }

    private void SetChildren() {
        Input = GetNode<InputComp>(nameof(InputComp));
        PlayerController = GetNode<PlayerVelocityAggregate>(nameof(PlayerVelocityAggregate));
        FloorDetector = GetNode<FloorDetector>(nameof(FloorDetector));
        WallDetector = GetNode<WallDetector>(nameof(WallDetector));
        
        GroundJumpParticles = GetNode<ImpactParticles>(nameof(GroundJumpParticles));
        AirJumpParticles = GetNode<ImpactParticles>(nameof(AirJumpParticles));
        LeftWallJumpParticles = GetNode<ImpactParticles>(nameof(LeftWallJumpParticles));
        RightWallJumpParticles = GetNode<ImpactParticles>(nameof(RightWallJumpParticles));
    }

    private void ConfigureInput() {
        Input.LeftInputOn += PlayerController.MoveLeft;
        Input.LeftInputOff += PlayerController.MoveRight;
        Input.RightInputOn += PlayerController.MoveRight;
        Input.RightInputOff += PlayerController.MoveLeft;
        Input.JumpInputOn += PlayerController.AttemptJump;
        Input.JumpInputOff += PlayerController.JumpCancel;
    }

    private void InitParticlesBehavior() {
        PlayerController.Jumping.Jumped += from => {
            ImpactParticles toEmit = (from, PlayerController.Jumping.JumpFacing) switch {
                (Location.Ground, _) => GroundJumpParticles,
                (Location.Air, _) => AirJumpParticles,
                (Location.WallNonGround, > 0) => LeftWallJumpParticles,
                (Location.WallNonGround, < 0) => RightWallJumpParticles,
                _ => null
            };
            toEmit?.EmitParticles();
        };
    }

    public override void _PhysicsProcess(double delta) {
        Velocity = PlayerController.Velocity;
		
        MoveAndSlideWithStatusChanges();
    }
}