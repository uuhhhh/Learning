using Godot;
using Learning.Scripts.Entity.Physics.Intermediate;

namespace Learning.Scripts.Environment;

/// <summary>
///     Data used by an EnvObject for modifying the movement of an entity that's on a wall.
/// </summary>
[GlobalClass]
public partial class WallData : Resource
{
    /// <summary>
    ///     Data used for modifying the wall dragging movement of the entity.
    /// </summary>
    [Export]
    public WallDraggingDataMultiplier WallDragging { get; private set; }

    /// <summary>
    ///     Data used for modifying the entity's wall jumping.
    /// </summary>
    [Export]
    public JumpingDataModifier WallJumping { get; private set; }

    /// <summary>
    ///     What priority this WallData should have over other WallDatas,
    ///     for selection by an EnvObjectDetector.
    /// </summary>
    [Export]
    public EnvironmentPriority Priority { get; private set; }
}