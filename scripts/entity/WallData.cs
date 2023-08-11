using Godot;

namespace Learning.scripts.entity;

[GlobalClass]
public partial class WallData : Resource
{
    [Export] public bool CanDrag { get; private set; }
    [Export] public float MaxDragSpeed { get; private set; }
    [Export] public float DragAscendGravityScale { get; private set; }
    [Export] public float DragDescendGravityScale { get; private set; }
    [Export] public float DragVelocityThresholdMin { get; private set; }
    [Export] public bool CanJump { get; private set; }
    [Export] public Vector2 JumpVelocity { get; private set; }
    [Export] public float JumpAcceleration { get; private set; }
    [Export] public float CoyoteJumpAcceleration { get; private set; }
}
