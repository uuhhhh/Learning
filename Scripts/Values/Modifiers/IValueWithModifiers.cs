using System;
using System.Collections.Generic;

namespace Learning.Scripts.Values.Modifiers;

/// <summary>
///     This class holds a base value and modifiers to apply to that base value,
///     to calculate a single modified value.
/// </summary>
/// <typeparam name="TValue">The type of the value being modified</typeparam>
public interface IValueWithModifiers<TValue>
{
    /// <summary>
    ///     The base value, after all modifiers have been applied.
    ///     First, the first modifier gets applied to the base value,
    ///     then the second modifier gets applied to the first modifier's calculated value, and so on.
    /// </summary>
    public TValue ModifiedValue { get; }

    /// <summary>
    ///     The value before any modifications, which gets passed into the first modifier.
    /// </summary>
    public TValue BaseValue { get; }

    /// <summary>
    ///     An event that gets invoked whenever a modifier is added.
    ///     The event argument is the modifier that was added.
    /// </summary>
    public event EventHandler<IModifier<TValue>> ModifierAdded;

    /// <summary>
    ///     An event that gets invoked whenever a modifier is removed.
    ///     The event argument is the modifier that was removed.
    /// </summary>
    public event EventHandler<IModifier<TValue>> ModifierRemoved;

    /// <summary>
    ///     An event that gets invoked whenever a modifier is added or removed.
    ///     The event argument is the modifier that was added or removed.
    /// </summary>
    public event EventHandler<IModifier<TValue>> ModifierUpdated;

    /// <summary>
    ///     Adds the given modifier, to the modifiers that this IValueWithModifiers applies to
    ///     values passed into ApplyModifier.
    /// </summary>
    /// <param name="modifier">The modifier to add</param>
    /// <returns>Whether the modifier was successfully added.</returns>
    public bool AddModifier(IModifier<TValue> modifier);

    /// <summary>
    ///     Removes the given modifier, from the modifiers that this IValueWithModifiers applies to
    ///     values passed into ApplyModifier.
    /// </summary>
    /// <param name="modifier">The modifier to remove</param>
    /// <returns>Whether the modifier was successfully remove.</returns>
    public bool RemoveModifier(IModifier<TValue> modifier);

    /// <summary>
    ///     Invalidates the currently-cached calculated modified value, if there is one.
    /// </summary>
    public void InvalidateCachedValue();

    /// <summary>
    ///     A class for comparing the priority values of IModifiers.
    /// </summary>
    public class PriorityComparer : IComparer<IModifier<TValue>>
    {
        public int Compare(IModifier<TValue> x, IModifier<TValue> y)
        {
            return (x, y) switch
            {
                (null, null) => 0,
                (null, _) => -1,
                (_, null) => 1,
                _ => x.Priority.PriorityNum().CompareTo(y.Priority.PriorityNum())
            };
        }
    }
}