using System;

namespace Learning.Scripts.Values.Modifiers;

/// <summary>
///     A modifier that applies an arbitrary function to given values.
/// </summary>
/// <typeparam name="TValue">The type of the value to apply modifications to.</typeparam>
public partial class FunctionalModifier<TValue> : Modifier<TValue>
{
    public FunctionalModifier(Func<TValue, TValue> func, ModifierPriority priority, bool cacheable)
        : base(priority, cacheable)
    {
        ToApplyModifierToFunc = func;
    }

    /// <summary>
    ///     The function that gets applied to passed-in values.
    /// </summary>
    public Func<TValue, TValue> ToApplyModifierToFunc { get; }

    /// <returns>ToApplyModifierToFunc applied to the given value.</returns>
    public override TValue ApplyModifier(TValue value)
    {
        return ToApplyModifierToFunc.Invoke(value);
    }
}