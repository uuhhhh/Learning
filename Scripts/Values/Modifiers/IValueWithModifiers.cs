﻿using System.Collections.Generic;

namespace Learning.Scripts.Values.Modifiers;

public interface IValueWithModifiers<TValue>
{
    public delegate void ModifiersUpdatedEventHandler();

    public TValue ModifiedValue { get; }

    public TValue BaseValue { get; }

    public event ModifiersUpdatedEventHandler ModifiersUpdated;

    public bool AddModifier(IModifier<TValue> value);

    public bool RemoveModifier(IModifier<TValue> value);

    public void InvalidateCachedValue();

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