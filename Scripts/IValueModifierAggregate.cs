using System.Collections.Generic;
using System.Collections.Immutable;

namespace Learning.Scripts; 

public interface IValueModifierAggregate {
    delegate void ModifiersUpdatedEventHandler();
    event ModifiersUpdatedEventHandler ModifiersUpdated;
    
    TValue ApplyModifiers<TValue>(string valueName, TValue initialValue) {
        TValue currentValue = initialValue;
        foreach (IValueModifier modifier in GetCurrentModifiers()) {
            currentValue = modifier.ApplyModifier(valueName, initialValue);
        }
        
        return currentValue;
    }

    bool AddModifiers(params IValueModifier[] modifiers);

    bool RemoveModifiers(params IValueModifier[] modifiers);

    IImmutableSet<IValueModifier> GetCurrentModifiers();
    
    protected static ISet<IValueModifier> DefaultModifierSetInit() {
        return new SortedSet<IValueModifier>(new PriorityComparer());
    }

    protected class PriorityComparer : IComparer<IValueModifier> {
        public int Compare(IValueModifier x, IValueModifier y) {
            return (x, y) switch {
                (null, null) => 0,
                (null, _) => -1,
                (_, null) => 1,
                _ => x.Priority.CompareTo(y.Priority)
            };
        }
    }
}