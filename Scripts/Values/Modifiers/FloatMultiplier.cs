using Godot;

namespace Learning.Scripts.Values.Modifiers;

/// <summary>
/// A modifier for multiplying float values.
/// </summary>
[GlobalClass]
public partial class FloatMultiplier : Modifier<float>
{
    /// <summary>
    /// The value to multiply passed-in values by when calculating modified values.
    /// </summary>
    [Export] public float Multiplier { get; private set; }

    /// <returns>The value multiplied by this FloatMultiplier's Multiplier.</returns>
    public override float ApplyModifier(float value)
    {
        return value * Multiplier;
    }
}