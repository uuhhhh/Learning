using System.Collections.Generic;

namespace Learning.Scripts; 

public interface IValueModifierAggregate {
    ISet<IValueModifier> Modifiers { get; }

    protected static ISet<IValueModifier> DefaultModifierInit() {
        return new SortedSet<IValueModifier>(new PriorityComparer());
    }

    TValue ApplyModifiers<TValue>(string valueName, TValue initialValue) {
        TValue currentValue = initialValue;
        foreach (IValueModifier modifier in Modifiers) {
            currentValue = modifier.ApplyModifier(valueName, initialValue);
        }

        return currentValue;
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