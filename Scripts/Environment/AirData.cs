using Godot;
using Learning.Scripts.Entity.Physics.Intermediate;
using Learning.Scripts.Entity.Physics.VelocitySources;

namespace Learning.Scripts.Environment;

/// <summary>
/// Data used by an EnvObject for modifying the movement of an entity that's in the air.
/// </summary>
[GlobalClass]
public partial class AirData : Resource
{
    /// <summary>
    /// Data used for modifying the falling movement of the entity.
    /// </summary>
    [Export] public FallingDataMultiplier AirFalling { get; private set; }
    
    /// <summary>
    /// Data used for modifying the horizontal movement of the entity when in the air.
    /// </summary>
    [Export] public LeftRightDataMultiplier AirMovement { get; private set; }
    
    /// <summary>
    /// Data used for modifying the entity's air jumping.
    /// </summary>
    [Export] public JumpingDataModifier AirJumping { get; private set; }
    
    /// <summary>
    /// What priority this AirData should have over other AirDatas,
    /// for selection by an EnvObjectDetector.
    /// </summary>
    [Export] public int Priority { get; private set; }
}