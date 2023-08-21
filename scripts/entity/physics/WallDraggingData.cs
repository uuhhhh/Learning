using Godot;

namespace Learning.scripts.entity.physics; 

[GlobalClass]
public partial class WallDraggingData : Resource {
    [Export] public FallingData DraggingData { get; private set; }
    [Export] public float VelocityDragThreshold { get; private set; }
}