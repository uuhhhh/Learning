using System;
using Godot;

namespace Learning.Scripts.Values.Modifiers;

/// <summary>
/// A modifier for applying a cap to integer values.
/// </summary>
[GlobalClass]
public partial class IntCapper : Modifier<int>
{
    /// <summary>
    /// The value to cap passed-in values at when calculating modified values.
    /// </summary>
    [Export] public int Cap { get; private set; }

    /// <returns>The given value capped at this IntCapper's Cap value
    /// (i.e., the min of the two values).</returns>
    public override int ApplyModifier(int value)
    {
        return Math.Min(value, Cap);
    }
}