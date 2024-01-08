using System;
using System.Collections.Generic;
using Godot;
using Learning.Scripts.Values.Modifiers;

namespace Learning.Scripts.Values.Groups;

/// <summary>
///     An abstract IValueWithModifiersGroup that's also a Godot Resource, so values for subclasses of
///     ResourceWithModifiers can specify values (presumably for the base values of the
///     IValueWithModifiers it will hold) in the Godot editor.
/// </summary>
public abstract partial class ResourceWithModifiers : Resource, IValueWithModifiersGroup
{
    private readonly IDictionary<string, object> _fields = new Dictionary<string, object>();

    protected ResourceWithModifiers()
    {
        ModifierAdded += (sender, s) => ModifierUpdated?.Invoke(sender, s);
        ModifierRemoved += (sender, s) => ModifierUpdated?.Invoke(sender, s);
    }

    public event EventHandler<string> ModifierAdded;
    public event EventHandler<string> ModifierRemoved;
    public event EventHandler<string> ModifierUpdated;

    public bool AddModifierTo<TValue>(string fieldName, IModifier<TValue> modifier)
    {
        bool modifierAdded = GetField<TValue, IValueWithModifiers<TValue>>(fieldName)
            .AddModifier(modifier);
        if (modifierAdded) ModifierAdded?.Invoke(this, fieldName);

        return modifierAdded;
    }

    public bool RemoveModifierFrom<TValue>(string fieldName, IModifier<TValue> modifier)
    {
        bool modifierRemoved = GetField<TValue, IValueWithModifiers<TValue>>(fieldName)
            .RemoveModifier(modifier);
        if (modifierRemoved) ModifierRemoved?.Invoke(this, fieldName);

        return modifierRemoved;
    }

    /// <summary>
    ///     Gets the modified value associated with the IValueWithModifiers of the given assigned name.
    /// </summary>
    /// <param name="fieldName">The assigned name of the IValueWithModifiers</param>
    /// <typeparam name="TValue">The type of value the IValueWithModifiers holds</typeparam>
    /// <returns>The modified value</returns>
    /// <exception cref="ArgumentException">
    ///     If TValue doesn't match
    ///     the type of value the IValueWithModifiers holds
    /// </exception>
    protected TValue GetValue<TValue>(string fieldName)
    {
        return GetField<TValue, IValueWithModifiers<TValue>>(fieldName).ModifiedValue;
    }

    /// <summary>
    ///     Creates a ValueWithModifiers and adds it to the IValueWithModifiers of this
    ///     ResourceWithModifiers.
    /// </summary>
    /// <param name="fieldName">The name that will be assigned to the ValueWithModifiers</param>
    /// <param name="value">The base value that the ValueWithModifiers will have</param>
    /// <typeparam name="TValue">The type of value the ValueWithModifiers will hold</typeparam>
    /// <exception cref="ArgumentException">
    ///     If the fieldName is one already assigned
    ///     to an IValueWithModifiers of this ResourceWithModifiers
    /// </exception>
    protected void InitValue<TValue>(string fieldName, TValue value)
    {
        if (_fields.ContainsKey(fieldName))
            throw new ArgumentException($"The name {fieldName} is already assigned to");
        _fields[fieldName] = new ValueWithModifiers<TValue>(value);
    }

    /// <summary>
    ///     Gets an IValueWithModifiers of this ResourceWithModifiers.
    /// </summary>
    /// <param name="fieldName">The name that was assigned to the IValueWithModifiers</param>
    /// <typeparam name="TValue">The type of value that the IValueWithModifiers holds</typeparam>
    /// <typeparam name="TValueWithModifiers">The type of the IValueWithModifiers itself</typeparam>
    /// <returns>The IValueWithModifiers</returns>
    /// <exception cref="ArgumentException">
    ///     If the given fieldName wasn't assigned to an
    ///     IValueWithModifiers, or if the IValueModifiers assigned to fieldName
    ///     is not a TValueWithModifiers.
    /// </exception>
    protected TValueWithModifiers GetField<TValue, TValueWithModifiers>(string fieldName)
        where TValueWithModifiers : IValueWithModifiers<TValue>
    {
        if (!_fields.ContainsKey(fieldName))
            throw new ArgumentException(
                $"{GetClass()} does not contain value for name {fieldName}");

        object untypedValue = _fields[fieldName];
        if (untypedValue is not TValueWithModifiers value)
            throw new ArgumentException(
                $"{GetClass()}'s {fieldName} value is not of type {typeof(TValue)}");

        return value;
    }

    /// <summary>
    ///     Adds the given IValueWithModifiers to this ResourceWithModifiers.
    /// </summary>
    /// <param name="fieldName">The name that will be assigned to the IValueWithModifiers</param>
    /// <param name="modifier">The IValueWithModifiers to be added</param>
    /// <typeparam name="TValue">The type of value the IValueWithModifiers holds</typeparam>
    /// <exception cref="ArgumentException">
    ///     If the fieldName is one already assigned
    ///     to an IValueWithModifiers of this ResourceWithModifiers
    /// </exception>
    protected void InitField<TValue>(string fieldName, IValueWithModifiers<TValue> modifier)
    {
        if (_fields.ContainsKey(fieldName))
            throw new ArgumentException($"The name {fieldName} is already assigned to");
        _fields[fieldName] = modifier;
    }
}