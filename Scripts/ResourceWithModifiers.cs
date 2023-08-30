using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using Godot;

namespace Learning.Scripts; 

public abstract partial class ResourceWithModifiers : Resource, IValueModifierAggregate {
    [Export] private bool CacheModifiedValues { get; set; } = true;
    public event IValueModifierAggregate.ModifiersUpdatedEventHandler ModifiersUpdated;
    
    private ISet<IValueModifier> Modifiers { get; } = IValueModifierAggregate.DefaultModifierSetInit();

    private readonly IDictionary<string, object> _backingFields = new Dictionary<string, object>();
    private readonly IDictionary<string, object> _modifiedFieldsCache = new Dictionary<string, object>();

    public bool AddModifier(IValueModifier modifier) {
        return AddModifier(modifier, true);
    }

    public bool AddModifier(IValueModifier modifier, bool refreshIfSuccessful) {
        bool successful = Modifiers.Add(modifier);

        if (successful && refreshIfSuccessful) {
            RefreshValues();
        }
        return successful;
    }
    
    public bool RemoveModifier(IValueModifier modifier) {
        return RemoveModifier(modifier, true);
    }

    public bool RemoveModifier(IValueModifier modifier, bool refreshIfSuccessful) {
        bool successful = Modifiers.Remove(modifier);

        if (successful && refreshIfSuccessful) {
            RefreshValues();
        }
        return successful;
    }

    public void RefreshValues() {
        RefreshAllFields();
        ModifiersUpdated?.Invoke();
    }

    public IImmutableSet<IValueModifier> GetCurrentModifiers() {
        return Modifiers.ToImmutableSortedSet();
    }

    protected TValue GetField<TValue>(string fieldName, bool alwaysRefreshValue = false) {
        if (!_backingFields.ContainsKey(fieldName)) {
            throw new ArgumentException($"No field with fieldName {fieldName}");
        }

        bool getCached = CacheModifiedValues && !alwaysRefreshValue && _modifiedFieldsCache.ContainsKey(fieldName);
        
        object value = getCached ? _modifiedFieldsCache[fieldName] : _backingFields[fieldName];
        if (value is not TValue valueAsT) {
            throw new ArgumentException($"Field with fieldName {fieldName} is not of given type param");
        }

        if (getCached) return valueAsT;
        
        TValue modifiedValue = ((IValueModifierAggregate) this).ApplyModifiers(fieldName, valueAsT);
        if (CacheModifiedValues) {
            _modifiedFieldsCache[fieldName] = modifiedValue;
        }
        return modifiedValue;
    }

    protected void SetField<TValue>(string fieldName, TValue value) {
        _backingFields[fieldName] = value;
        
        RefreshField<TValue>(fieldName);
    }

    protected void RefreshField<TValue>(string fieldName) {
        GetField<TValue>(fieldName, alwaysRefreshValue: true);
    }

    protected abstract void RefreshAllFields();
}