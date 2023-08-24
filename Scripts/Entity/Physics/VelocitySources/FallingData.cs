using Godot;

namespace Learning.Scripts.Entity.Physics.VelocitySources; 

[GlobalClass]
public partial class FallingData : Resource {
    [Export] public float UpwardsGravityScale { get; private set; }
    [Export] public float DownwardsGravityScale { get; private set; }
    [Export] public float MaxVelocity { get; private set; }
    [Export] public float CeilingHitStopTimeScale { get; private set; }
    [Export] public float DecelToMaxVelocityTimePerVelocity { get; private set; }
}