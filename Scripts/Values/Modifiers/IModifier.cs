namespace Learning.Scripts.Values.Modifiers;

/// <summary>
///     An interface for applying some modification to a given value.
/// </summary>
/// <typeparam name="TValue">The type of the value to apply modifications to.</typeparam>
public interface IModifier<TValue>
{
    /// <summary>
    ///     When this modifier should be applied relative to other IModifiers.
    ///     Lower priority values mean this modifier gets applied later.
    /// </summary>
    /// <see cref="ModifierPriorityExtensions.PriorityNum" />
    public ModifierPriority Priority { get; }

    /// <summary>
    ///     Whether calculated modified values can be cached,
    ///     to skip recalculation on subsequent ApplyModifier calls.
    /// </summary>
    public bool Cacheable { get; }

    /// <summary>
    ///     Calculate a modified value based on the given value.
    /// </summary>
    /// <param name="value">The value to find a modified value from</param>
    /// <returns>The modified value</returns>
    public TValue ApplyModifier(TValue value);
}