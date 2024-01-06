using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace Learning.Scripts.Values.Modifiers;

/// <summary>
///     A basic concrete implementation of IValueWithModifiers.
/// </summary>
/// <typeparam name="TValue">The type of the value being modified</typeparam>
public class ValueWithModifiers<TValue> : IValueWithModifiers<TValue>
{
    private readonly ISet<IModifier<TValue>> _currentModifiers
        = new SortedSet<IModifier<TValue>>(new IValueWithModifiers<TValue>.PriorityComparer());

    private TValue _baseValue;
    private TValue _cachedValue;
    private bool _currentCachedValueValid;
    private int _numNonCacheableModifiers;

    /// <summary>
    ///     Creates a ValueWithModifiers with currently no modifiers.
    /// </summary>
    /// <param name="baseValue">The base value for this ValueWithModifiers</param>
    public ValueWithModifiers(TValue baseValue)
    {
        BaseValue = baseValue;
        _cachedValue = BaseValue;
        _currentCachedValueValid = true;
        _numNonCacheableModifiers = 0;

        ModifierAdded += (sender, modifier) => ModifierUpdated?.Invoke(sender, modifier);
        ModifierRemoved += (sender, modifier) => ModifierUpdated?.Invoke(sender, modifier);
    }

    /// <summary>
    ///     An immutable view of the current modifiers of this ValueWithModifiers.
    /// </summary>
    public IImmutableSet<IModifier<TValue>> CurrentModifiers =>
        _currentModifiers.ToImmutableSortedSet();

    public event EventHandler<IModifier<TValue>> ModifierAdded;

    public event EventHandler<IModifier<TValue>> ModifierRemoved;

    public event EventHandler<IModifier<TValue>> ModifierUpdated;

    public TValue ModifiedValue =>
        _currentCachedValueValid ? _cachedValue : ApplyModifiersTo(BaseValue);

    public TValue BaseValue
    {
        get => _baseValue;
        set
        {
            _baseValue = value;
            InvalidateCachedValue();
        }
    }

    public bool AddModifier(IModifier<TValue> modifier)
    {
        bool modifierAdded = _currentModifiers.Add(modifier);

        if (modifierAdded)
        {
            _numNonCacheableModifiers += modifier.Cacheable ? 0 : 1;
            InvalidateCachedValue();
            ModifierAdded?.Invoke(this, modifier);
        }

        return modifierAdded;
    }

    public bool RemoveModifier(IModifier<TValue> modifier)
    {
        bool modifierRemoved = _currentModifiers.Remove(modifier);

        if (modifierRemoved)
        {
            _numNonCacheableModifiers -= modifier.Cacheable ? 0 : 1;
            InvalidateCachedValue();
            ModifierRemoved?.Invoke(this, modifier);
        }

        return modifierRemoved;
    }

    public void InvalidateCachedValue()
    {
        _currentCachedValueValid = false;
        _cachedValue = default;
    }

    private TValue ApplyModifiersTo(TValue initialValue)
    {
        TValue modifiedValue = CurrentModifiers.Aggregate(
            initialValue,
            (value, modifier) => modifier.ApplyModifier(value),
            value => value);

        bool canCacheValue = _numNonCacheableModifiers == 0;
        _cachedValue = canCacheValue ? modifiedValue : default;
        _currentCachedValueValid = canCacheValue;

        return modifiedValue;
    }
}