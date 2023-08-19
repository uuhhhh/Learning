using Godot;

namespace Learning.scripts.entity.physics; 

[GlobalClass]
public partial class JumpingData : Resource {
    public const int UNLIMITED_JUMPS = -1;
    
    [Export] public int NumJumps { get; private set; }
    [Export] public Vector2 Velocity { get; private set; }
    [Export] public float AccelTimeX { get; private set; }
    [Export] public float AccelTimeY { get; private set; }
    [Export] public float CancelVelocity { get; private set; }
    [Export] public float CancelAccelTime { get; private set; }
}