using Godot;

namespace Learning.Scripts.Values.Modifiers;

[GlobalClass]
public abstract partial class ModifierWithModifiers<TValue> : Resource, IValueWithModifiers<TValue>,
    IModifier<TValue>
{
    private readonly ValueWithModifiers<TValue> _backing = new(default);
    [Export] public ModifierPriority Priority { get; private set; }
    [Export] public bool Cacheable { get; private set; }

    public abstract TValue ApplyModifier(TValue value);
    public event IValueWithModifiers<TValue>.ModifiersUpdatedEventHandler ModifiersUpdated;

    public TValue ModifiedValue => _backing.ModifiedValue;

    public TValue BaseValue
    {
        get => _backing.BaseValue;
        set => _backing.BaseValue = value;
    }

    public bool AddModifier(IModifier<TValue> value)
    {
        return _backing.AddModifier(value);
    }

    public bool RemoveModifier(IModifier<TValue> value)
    {
        return _backing.RemoveModifier(value);
    }

    public void InvalidateCachedValue()
    {
        _backing.InvalidateCachedValue();
    }
}