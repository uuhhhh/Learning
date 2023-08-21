using Godot;
using Learning.Scripts.Entity.Physics.VelocitySources;

namespace Learning.Scripts.Entity.Physics.Intermediate; 

[GlobalClass]
public partial class WallDraggingData : Resource {
    [Export] public FallingData DraggingData { get; private set; }
    [Export] public float VelocityDragThreshold { get; private set; }
}