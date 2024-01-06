using Godot;

namespace Learning.Scripts.Environment;

/// <summary>
/// An EnvObject can modify the player (or an entity with an EnvObjectDetector)'s
/// movement when they enter its vicinity. The modifications are based on the data given to this
/// EnvObject.
/// </summary>
public partial class EnvObject : Node2D
{
    /// <summary>
    /// Data used for modifying the ground, air, and wall movement of the entity.
    /// </summary>
    [Export] public EnvObjectData Data { get; private set; }
}