using System;
using Godot;

namespace Learning.Scripts.Values.Modifiers;

/// <summary>
/// This class holds a base value and modifiers to apply to that base value, and this class has
/// a method to modify a given value (presumably using this class' ModifiedValue).
/// </summary>
/// <typeparam name="TValue">The type of the base/modified value held by this class,
/// and the type of the value modified by this class' ApplyModifier</typeparam>
[GlobalClass]
public abstract partial class ModifierWithModifiers<TValue> : Resource, IValueWithModifiers<TValue>,
    IModifier<TValue>
{
    private readonly ValueWithModifiers<TValue> _backing = new(default);
    [Export] public ModifierPriority Priority { get; private set; }
    [Export] public bool Cacheable { get; private set; }

    public abstract TValue ApplyModifier(TValue value);
    
    public event EventHandler<IModifier<TValue>> ModifierAdded;
    public event EventHandler<IModifier<TValue>> ModifierRemoved;

    public TValue ModifiedValue => _backing.ModifiedValue;

    protected ModifierWithModifiers()
    {
        _backing.ModifierAdded += (sender, modifier) => ModifierAdded?.Invoke(sender, modifier);
        _backing.ModifierRemoved += (sender, modifier) => ModifierRemoved?.Invoke(sender, modifier);
    }

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