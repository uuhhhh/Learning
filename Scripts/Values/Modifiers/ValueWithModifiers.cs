using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace Learning.Scripts.Values.Modifiers; 

public class ValueWithModifiers<TValue> : IValueWithModifiers<TValue> {
    public event IValueWithModifiers<TValue>.ModifiersUpdatedEventHandler ModifiersUpdated;
    
    public TValue ModifiedValue => _currentCachedValueValid ? _cachedValue : ApplyModifiersTo(BaseValue);
    
    public IImmutableSet<IModifier<TValue>> CurrentModifiers => _currentModifiers.ToImmutableSortedSet();

    public TValue BaseValue {
        get => _baseValue;
        set {
            _baseValue = value;
            InvalidateCachedValue();
        }
    }

    private readonly ISet<IModifier<TValue>> _currentModifiers
        = new SortedSet<IModifier<TValue>>(new IValueWithModifiers<TValue>.PriorityComparer());

    private TValue _baseValue;
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

    public bool AddModifier(IModifier<TValue> modifier) {
        bool modifierAdded = _currentModifiers.Add(modifier);

        if (modifierAdded) {
            _numNonCacheableModifiers += modifier.Cacheable ? 0 : 1;
            InvalidateCachedValue();
            ModifiersUpdated?.Invoke();
        }

        return modifierAdded;
    }

    public bool RemoveModifier(IModifier<TValue> modifier) {
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
}