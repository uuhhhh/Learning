using Godot;
using Learning.Scripts.Entity.Physics.Intermediate;
using Learning.Scripts.Entity.Physics.VelocitySources;

namespace Learning.Scripts.Environment;

public partial class FloorData : Resource
{
    [Export] public LeftRightDataMultiplier FloorMovement { get; private set; }
    [Export] public JumpingDataModifier FloorJumping { get; private set; }
    [Export] public int FloorDefaultPriority { get; private set; }
}