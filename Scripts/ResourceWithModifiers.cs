using System;
using System.Collections.Generic;
using Godot;

namespace Learning.Scripts; 

public abstract partial class ResourceWithModifiers : Resource, IValueModifierAggregate {
    public ISet<IValueModifier> Modifiers { get; } = IValueModifierAggregate.DefaultModifierInit();

    private readonly IDictionary<string, object> _backingFields = new Dictionary<string, object>();

    protected TValue GetField<TValue>(string fieldName) {
        if (!_backingFields.ContainsKey(fieldName)) {
            throw new ArgumentException($"No field with fieldName {fieldName}");
        }
        
        object value = _backingFields[fieldName];
        if (value is not TValue valueAsT) {
            throw new ArgumentException($"Field with fieldName {fieldName} is not of given type param");
        }
        return valueAsT;
    }

    protected void SetField(string fieldName, object value) {
        _backingFields[fieldName] = value;
    }
}