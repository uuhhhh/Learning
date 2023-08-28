using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using Godot;

namespace Learning.Scripts; 

public abstract partial class ResourceWithModifiers : Resource, IValueModifierAggregate {
    public event IValueModifierAggregate.ModifiersUpdatedEventHandler ModifiersUpdated;
    
    private ISet<IValueModifier> Modifiers { get; } = IValueModifierAggregate.DefaultModifierSetInit();

    private readonly IDictionary<string, object> _backingFields = new Dictionary<string, object>();

    public bool AddModifiers(params IValueModifier[] modifiers) {
        bool anyAdded = false;
        foreach (IValueModifier modifier in modifiers) {
            anyAdded |= Modifiers.Add(modifier);
        }

        if (anyAdded) {
            UpdateModifiers();
        }
        return anyAdded;
    }

    public bool RemoveModifiers(params IValueModifier[] modifiers) {
        bool anyRemoved = false;
        foreach (IValueModifier modifier in modifiers) {
            anyRemoved |= Modifiers.Remove(modifier);
        }

        if (anyRemoved) {
            UpdateModifiers();
        }
        return anyRemoved;
    }

    public void UpdateModifiers() {
        ModifiersUpdated?.Invoke();
    }

    public IImmutableSet<IValueModifier> GetCurrentModifiers() {
        return Modifiers.ToImmutableSortedSet();
    }

    protected TValue GetField<TValue>(string fieldName) {
        if (!_backingFields.ContainsKey(fieldName)) {
            throw new ArgumentException($"No field with fieldName {fieldName}");
        }
        
        object value = _backingFields[fieldName];
        if (value is not TValue initialValue) {
            throw new ArgumentException($"Field with fieldName {fieldName} is not of given type param");
        }
        return ((IValueModifierAggregate) this).ApplyModifiers(fieldName, initialValue);
    }

    protected void SetField(string fieldName, object value) {
        _backingFields[fieldName] = value;
    }
}