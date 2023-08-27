using Godot;
using Learning.Scripts.Entity.Physics.Intermediate;

namespace Learning.Scripts.Entity.Physics; 

public partial class WallData : Resource {
    [Export] public WallDraggingDataMultiplier WallDragging { get; private set; }
    [Export] public JumpingDataModifier WallJumping { get; private set; }
    [Export] public int WallDefaultPriority { get; private set; }
}