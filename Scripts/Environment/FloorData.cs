using Godot;
using Learning.Scripts.Entity.Physics.Intermediate;
using Learning.Scripts.Entity.Physics.VelocitySources;

namespace Learning.Scripts.Environment;

/// <summary>
///     Data used by an EnvObject for modifying the movement of an entity that's on the ground.
/// </summary>
[GlobalClass]
public partial class FloorData : Resource
{
    /// <summary>
    ///     Data used for modifying the horizontal movement of the entity when on the ground.
    /// </summary>
    [Export]
    public LeftRightDataMultiplier FloorMovement { get; private set; }

    /// <summary>
    ///     Data used for modifying the entity's ground jumping.
    /// </summary>
    [Export]
    public JumpingDataModifier FloorJumping { get; private set; }

    /// <summary>
    ///     What priority this FloorData should have over other FloorDatas,
    ///     for selection by an EnvObjectDetector.
    /// </summary>
    [Export]
    public int Priority { get; private set; }
}