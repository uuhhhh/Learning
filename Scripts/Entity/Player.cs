using Godot;
using Learning.Scripts.Effects;
using Learning.Scripts.Entity.Physics;

namespace Learning.Scripts.Entity; 

public partial class Player : KinematicComp {
    public InputComp Input { get; private set; }
    public PlayerVelocityAggregate PlayerController { get; private set; }
    public FloorDetector FloorDetector { get; private set; }
    public WallDetector WallDetector { get; private set; }
    private Node Particles { get; set; }
    public ImpactParticles GroundJumpParticles { get; private set; }
    public ImpactParticles AirJumpParticles { get; private set; }
    public ImpactParticles LeftWallJumpParticles { get; private set; }
    public ImpactParticles RightWallJumpParticles { get; private set; }
    public CpuParticles2D WalkingParticles { get; private set; }
    public CpuParticles2D LeftWallDragParticles { get; private set; }
    public CpuParticles2D RightWallDragParticles { get; private set; }

    public override void _Ready() {
        SetChildren();
        PlayerController.InitializeInteractions(this);
        base._Ready();

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

        Particles = GetNode<Node>(nameof(Particles));
        GroundJumpParticles = Particles.GetNode<ImpactParticles>(nameof(GroundJumpParticles));
        AirJumpParticles = Particles.GetNode<ImpactParticles>(nameof(AirJumpParticles));
        LeftWallJumpParticles = Particles.GetNode<ImpactParticles>(nameof(LeftWallJumpParticles));
        RightWallJumpParticles = Particles.GetNode<ImpactParticles>(nameof(RightWallJumpParticles));
        WalkingParticles = Particles.GetNode<CpuParticles2D>(nameof(WalkingParticles));
        LeftWallDragParticles = Particles.GetNode<CpuParticles2D>(nameof(LeftWallDragParticles));
        RightWallDragParticles = Particles.GetNode<CpuParticles2D>(nameof(RightWallDragParticles));
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
        InitJumpParticleBehavior();
        InitWalkParticleBehavior();
        InitWallDragParticleBehavior();
    }

    private void InitJumpParticleBehavior() {
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

    private void InitWalkParticleBehavior() {
        PlayerController.LeftRight.GoingLeft += WalkCheck;
        PlayerController.LeftRight.GoingRight += WalkCheck;
        PlayerController.LeftRight.StopMoving += WalkCheck;
        BecomeOnFloor += _ => WalkCheck();
        BecomeOffFloor += _ => WalkCheck();
        BecomeOnWall += _ => WalkCheck();
        BecomeOffWall += _ => WalkCheck();
    }

    private void InitWallDragParticleBehavior() {
        PlayerController.WallDragging.StartedDragging += () => {
            CpuParticles2D toEmit = GetWallNormal().X switch {
                > 0 => LeftWallDragParticles,
                < 0 => RightWallDragParticles,
                _ => null
            };
            if (toEmit is not null) {
                toEmit.Emitting = true;
            }
        };
        PlayerController.WallDragging.StoppedDragging += () => {
            LeftWallDragParticles.Emitting = false;
            RightWallDragParticles.Emitting = false;
        };
    }

    private void WalkCheck() {
        WalkingParticles.Emitting = IsOnFloor() && !IsOnWall() && PlayerController.LeftRight.CurrentSpeedScale != 0;
    }

    public override void _PhysicsProcess(double delta) {
        Velocity = PlayerController.Velocity;
		
        MoveAndSlideWithStatusChanges();
    }
}