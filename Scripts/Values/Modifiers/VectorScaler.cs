using Godot;

namespace Learning.Scripts.Values.Modifiers;

/// <summary>
/// A modifier for scaling Vector2 values.
/// </summary>
[GlobalClass]
public partial class VectorScaler : Modifier<Vector2>
{
    /// <summary>
    /// The value to multiply (both components of) passed-in values by
    /// when calculating modified values.
    /// </summary>
    [Export] public Vector2 Scale { get; private set; }

    /// <returns>The value with both components scaled by this VectorScaler's Scale.</returns>
    public override Vector2 ApplyModifier(Vector2 value)
    {
        return Scale * value;
    }
}