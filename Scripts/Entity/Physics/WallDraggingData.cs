using Godot;

namespace Learning.Scripts.Entity.Physics; 

[GlobalClass]
public partial class WallDraggingData : Resource {
    [Export] public FallingData DraggingData { get; private set; }
    [Export] public float VelocityDragThreshold { get; private set; }
}