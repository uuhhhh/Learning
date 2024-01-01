using System;
using System.Collections.Generic;
using Godot;

namespace Learning.Scripts.Values; 

public abstract partial class ResourceWithModifiers : Resource, IValueWithModifiersGroup {
    public event IValueWithModifiersGroup.ModifiersUpdatedEventHandler ModifiersUpdated;
    
    private readonly IDictionary<string, object> _fields = new Dictionary<string, object>();

    protected TValue GetValue<TValue>(string fieldName) {
        return GetField<TValue>(fieldName).ModifiedValue;
    }
    
    protected void InitValue<TValue>(string fieldName, TValue value) {
        _fields[fieldName] = new ValueWithModifiers<TValue>(value);
    }

    public bool AddModifierTo<TValue>(string fieldName, Modifier<TValue> modifier) {
        bool modifierAdded = GetField<TValue>(fieldName).AddModifier(modifier);
        if (modifierAdded) {
            ModifiersUpdated?.Invoke();
        }

        return modifierAdded;
    }

    public bool RemoveModifierFrom<TValue>(string fieldName, Modifier<TValue> modifier) {
        bool modifierRemoved = GetField<TValue>(fieldName).RemoveModifier(modifier);
        if (modifierRemoved) {
            ModifiersUpdated?.Invoke();
        }

        return modifierRemoved;
    }

    private ValueWithModifiers<TValue> GetField<TValue>(string fieldName) {
        if (!_fields.ContainsKey(fieldName)) {
            throw new ArgumentException($"{GetClass()} does not contain value for name {fieldName}");
        }

        object untypedValue = _fields[fieldName];
        if (untypedValue is not ValueWithModifiers<TValue> value) {
            throw new ArgumentException($"{GetClass()}'s {fieldName} value is not of type {typeof(TValue)}");
        }
        
        return value;
    }
}