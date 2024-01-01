using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace Learning.Scripts.Values; 

public class ValueWithModifiers<TValue> {
    public TValue ModifiedValue => _currentCachedValueValid ? _cachedValue : ApplyModifiersTo(BaseValue);
    
    public IImmutableSet<Modifier<TValue>> CurrentModifiers => _currentModifiers.ToImmutableSortedSet();

    public readonly TValue BaseValue;

    private readonly ISet<Modifier<TValue>> _currentModifiers
        = new SortedSet<Modifier<TValue>>(new PriorityComparer());
    
    public delegate void ModifiersUpdatedEventHandler();
    public event ModifiersUpdatedEventHandler ModifiersUpdated;

    private TValue _cachedValue;
    private int _numNonCacheableModifiers;
    private bool _currentCachedValueValid;

    public ValueWithModifiers(TValue baseValue) {
        BaseValue = baseValue;
        _cachedValue = BaseValue;
        _currentCachedValueValid = true;
        _numNonCacheableModifiers = 0;
    }
    
    private TValue ApplyModifiersTo(TValue initialValue) {
        TValue modifiedValue = CurrentModifiers.Aggregate(
            initialValue,
            (value, modifier) => modifier.ApplyModifier(value),
            value => value);

        bool canCacheValue = _numNonCacheableModifiers == 0;
        _cachedValue = canCacheValue ? modifiedValue : default;
        _currentCachedValueValid = canCacheValue;
        
        return modifiedValue;
    }

    public bool AddModifier(Modifier<TValue> modifier) {
        bool modifierAdded = _currentModifiers.Add(modifier);

        if (modifierAdded) {
            _numNonCacheableModifiers += modifier.Cacheable ? 0 : 1;
            InvalidateCachedValue();
            ModifiersUpdated?.Invoke();
        }

        return modifierAdded;
    }

    public bool RemoveModifier(Modifier<TValue> modifier) {
        bool modifierRemoved = _currentModifiers.Remove(modifier);

        if (modifierRemoved) {
            _numNonCacheableModifiers -= modifier.Cacheable ? 0 : 1;
            InvalidateCachedValue();
            ModifiersUpdated?.Invoke();
        }

        return modifierRemoved;
    }

    public void InvalidateCachedValue() {
        _currentCachedValueValid = false;
        _cachedValue = default;
    }

    private class PriorityComparer : IComparer<Modifier<TValue>> {
        public int Compare(Modifier<TValue> x, Modifier<TValue> y) {
            return (x, y) switch {
                (null, null) => 0,
                (null, _) => -1,
                (_, null) => 1,
                _ => x.Priority.CompareTo(y.Priority)
            };
        }
    }
}