using Godot;

namespace Learning.scripts.entity.physics; 

[GlobalClass]
public partial class FallingData : Resource {
    [Export] public float UpwardsGravityScale { get; private set; }
    [Export] public float DownwardsGravityScale { get; private set; }
    [Export] public float MaxVelocity { get; private set; }
    [Export] public float CeilingHitStopTimeScale { get; private set; }
}