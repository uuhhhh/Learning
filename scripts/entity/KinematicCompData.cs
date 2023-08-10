using Godot;

namespace Learning.scripts.entity;

[GlobalClass]
public partial class KinematicCompData : Resource {
    public const int UnlimitedAirJumps = -1;
    
    [Export] public float Speed { get; private set; }
    [Export] public float JumpVelocity { get; private set; }
    [Export] public float JumpCancelVelocityProportion { get; private set; }
    [Export] public float AirJumpVelocity { get; private set; }
    [Export] public int NumAirJumps { get; private set; }
    [Export] public Vector2 WallJumpVelocity { get; private set; }
    [Export] public bool CanWallJump { get; private set; }
    [Export] public float WallDragSpeedMax { get; private set; }
    [Export] public float WallDragAccelRatio { get; private set; }
    [Export] public float WallDragVelocityThresholdMin { get; private set; }
    [Export] public bool CanWallDrag { get; private set; }
    [Export] public float GroundAcceleration { get; private set; }
    [Export] public float AirAcceleration { get; private set; }
    [Export] public float GroundDeceleration { get; private set; }
    [Export] public float AirDeceleration { get; private set; }
    [Export] public float TurnaroundAccelerationDampening { get; private set; }
    [Export] public float UpwardsGravityScale { get; private set; }
    [Export] public float DownwardsGravityScale { get; private set; }
}
