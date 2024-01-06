using Godot;

namespace Learning.Scripts.Values.Modifiers;

/// <summary>
///     A modifier for replacing a float with a different float (that had modifiers applied to it).
/// </summary>
[GlobalClass]
public partial class ModifiedFloatReplacer : ModifierWithModifiers<float>
{
    // this class isn't generic since Godot's Exports don't like generic values

    /// <summary>
    ///     The base float replacement value that modifiers get applied to.
    /// </summary>
    [Export]
    public new float BaseValue
    {
        get => base.BaseValue;
        set => base.BaseValue = value;
    }

    /// <returns>This ModifiedFloatReplacer's ModifiedValue. The given value is ignored.</returns>
    public override float ApplyModifier(float value)
    {
        return ModifiedValue;
    }
}