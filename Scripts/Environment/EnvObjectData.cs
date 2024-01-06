using Godot;

namespace Learning.Scripts.Environment;

/// <summary>
///     Data used for modifying the movement of a player (or an entity with an EnvObjectDetector)
///     that has entered the vicinity of an EnvObject.
/// </summary>
[GlobalClass]
public partial class EnvObjectData : Resource
{
    /// <summary>
    ///     Ground movement modification data for this environment object.
    /// </summary>
    [Export]
    public FloorData Floor { get; private set; }

    /// <summary>
    ///     Air movement modification data for this environment object.
    /// </summary>
    [Export]
    public AirData Air { get; private set; }

    /// <summary>
    ///     Wall movement modification data for this environment object.
    /// </summary>
    [Export]
    public WallData Wall { get; private set; }
}