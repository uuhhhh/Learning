using Godot;
using Learning.Scripts.Entity.Physics.Intermediate;
using Learning.Scripts.Entity.Physics.VelocitySources;

namespace Learning.Scripts.Entity.Physics;

public partial class AirData : Resource
{
    [Export] public FallingDataMultiplier AirFalling { get; private set; }
    [Export] public LeftRightDataMultiplier AirMovement { get; private set; }
    [Export] public JumpingDataModifier AirJumping { get; private set; }
    [Export] public int AirDefaultPriority { get; private set; }
}