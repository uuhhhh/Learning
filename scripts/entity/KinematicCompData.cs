using Godot;

namespace Learning.scripts.entity;

[GlobalClass]
public partial class KinematicCompData : Resource {
    [Export] public float Speed { get; set; }
    [Export] public float JumpVelocity { get; set; }
    [Export] public float JumpCancelVelocityProportion { get; set; }
    [Export] public float GroundAcceleration { get; set; }
    [Export] public float AirAcceleration { get; set; }
    [Export] public float GroundDeceleration { get; set; }
    [Export] public float AirDeceleration { get; set; }
    [Export] public float UpwardsGravityScale { get; set; }
    [Export] public float DownwardsGravityScale { get; set; }
    [Export] public float TurnaroundAccelerationDampening { get; set; }
}
