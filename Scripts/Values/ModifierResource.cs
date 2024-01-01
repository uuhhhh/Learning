using System;
using System.Collections.Generic;
using Godot;

namespace Learning.Scripts.Values; 

public abstract partial class ModifierResource<TValues>
    : Resource, IModifierGroup<TValues> where TValues : ResourceWithModifiers {
    private readonly IDictionary<string, object> _modifiers = new Dictionary<string, object>();
    
    private delegate void ModifierEventHandler(ResourceWithModifiers resources);
    
    private event ModifierEventHandler AddingModifiers;
    private event ModifierEventHandler RemovingModifiers;

    protected void AddModifierToBeAdded<TValue>(string modifierName, string fieldName, Modifier<TValue> modifier) {
        AddingModifiers += values => values.AddModifierTo(fieldName, modifier);
        RemovingModifiers += values => values.RemoveModifierFrom(fieldName, modifier);
        _modifiers[modifierName] = modifier;
    }

    protected TModifier GetModifier<TModifier>(string modifierName) {
        if (!_modifiers.ContainsKey(modifierName)) {
            throw new ArgumentException($"{GetClass()} does not contain modifier for name {modifierName}");
        }

        object untypedModifier = _modifiers[modifierName];
        if (untypedModifier is not TModifier modifier) {
            throw new ArgumentException($"{GetClass()}'s {modifierName} is not of type {typeof(TModifier)}");
        }

        return modifier;
    }

    public void AddModifiersTo(TValues toAddModifiersTo) {
        AddingModifiers?.Invoke(toAddModifiersTo);
    }

    public void RemoveModifiersFrom(TValues toRemoveModifiersFrom) {
        RemovingModifiers?.Invoke(toRemoveModifiersFrom);
    }
}