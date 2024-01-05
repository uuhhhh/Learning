using Learning.Scripts.Effects;
using Learning.Scripts.Entity.Physics;
using Learning.Scripts.Environment;

namespace Learning.Scripts.Entity;

/// <summary>
/// The "brain" of the player,
/// governing the behaviors and interactions between its different components.
/// </summary>
public partial class Player : KinematicComp
{
    /// <summary>
    /// The inputs that control the player.
    /// </summary>
    public InputComp Input { get; private set; }
    
    /// <summary>
    /// The component that determines the player's movement.
    /// </summary>
    public PlayerVelocityAggregate PlayerController { get; private set; }
    
    /// <summary>
    /// The component that detects floors by the player.
    /// </summary>
    public FloorDetector FloorDetector { get; private set; }
    
    /// <summary>
    /// The component that detects walls by the player.
    /// </summary>
    public WallDetector WallDetector { get; private set; }
    
    /// <summary>
    /// Particle effects emitted by the player.
    /// </summary>
    public PlayerParticles Particles { get; private set; }

    public override void _Ready()
    {
        SetChildren();
        PlayerController.InitializeInteractions(this);
        base._Ready();

        ConfigureInput();

        Particles.InitParticlesBehavior(this);

        WallDetector.ZeroToOneEnvObjects += () => PlayerController.CanDoWallBehavior = true;
        WallDetector.ZeroEnvObjects += () => PlayerController.CanDoWallBehavior = false;
    }

    private void SetChildren()
    {
        Input = GetNode<InputComp>(nameof(InputComp));
        PlayerController = GetNode<PlayerVelocityAggregate>(nameof(PlayerVelocityAggregate));
        FloorDetector = GetNode<FloorDetector>(nameof(FloorDetector));
        WallDetector = GetNode<WallDetector>(nameof(WallDetector));
        Particles = GetNode<PlayerParticles>(nameof(Particles));
    }

    private void ConfigureInput()
    {
        Input.LeftInputOn += PlayerController.MoveLeft;
        Input.LeftInputOff += PlayerController.MoveRight;
        Input.RightInputOn += PlayerController.MoveRight;
        Input.RightInputOff += PlayerController.MoveLeft;
        Input.JumpInputOn += PlayerController.AttemptJump;
        Input.JumpInputOff += PlayerController.JumpCancel;
    }

    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);
        Velocity = PlayerController.Velocity;

        MoveAndSlideWithStatusChanges();
    }
}