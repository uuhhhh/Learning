using Godot;

namespace Learning.Scripts.Values.Modifiers; 

[GlobalClass]
public abstract partial class ModifierWithModifiers<TValue> : Resource, IValueWithModifiers<TValue>, IModifier<TValue> {
    public event IValueWithModifiers<TValue>.ModifiersUpdatedEventHandler ModifiersUpdated;
    [Export] public int Priority { get; private set; }
    [Export] public bool Cacheable { get; private set; }

    public TValue ModifiedValue => _backing.ModifiedValue;

    public TValue BaseValue {
        get => _backing.BaseValue;
        set => _backing.BaseValue = value;
    }

    private readonly ValueWithModifiers<TValue> _backing = new(default);
    
    public bool AddModifier(IModifier<TValue> value) {
        return _backing.AddModifier(value);
    }

    public bool RemoveModifier(IModifier<TValue> value) {
        return _backing.RemoveModifier(value);
    }

    public void InvalidateCachedValue() {
        _backing.InvalidateCachedValue();
    }

    public abstract TValue ApplyModifier(TValue value);
}