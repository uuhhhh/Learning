using System;
using System.Collections.Generic;
using Godot;
using Learning.Scripts.Values.Modifiers;

namespace Learning.Scripts.Values.Groups;

/// <summary>
///     An abstract IModifierGroup that's also a Godot Resource, so values for subclasses of
///     ModifierResource can specify values (presumably for the IModifiers it will hold)
///     in the Godot editor.
/// </summary>
/// <typeparam name="TValues">
///     The type of IValueWithModifiersGroup that IModifiers
///     of this ModifierResource will affect
/// </typeparam>
public abstract partial class ModifierResource<TValues>
    : Resource, IModifierGroup<TValues> where TValues : IValueWithModifiersGroup
{
    private readonly IDictionary<string, object> _modifiers = new Dictionary<string, object>();

    public void AddModifiersTo(TValues toAddModifiersTo)
    {
        AddingModifiers?.Invoke(toAddModifiersTo);
    }

    public void RemoveModifiersFrom(TValues toRemoveModifiersFrom)
    {
        RemovingModifiers?.Invoke(toRemoveModifiersFrom);
    }

    private event Action<IValueWithModifiersGroup> AddingModifiers;
    private event Action<IValueWithModifiersGroup> RemovingModifiers;

    /// <summary>
    ///     Adds an IModifier to this ModifierResource.
    /// </summary>
    /// <param name="modifierName">The name that will be assigned to this IModifier</param>
    /// <param name="fieldName">
    ///     The assigned name of the IValueWithModifiers in TValues
    ///     that this IModifier will modify
    /// </param>
    /// <param name="modifier">The modifier to be added</param>
    /// <typeparam name="TValue">The type of value that the modifier modifies</typeparam>
    /// <exception cref="ArgumentException">
    ///     If the modifierName is one already assigned
    ///     to an IModifier of this ModifierResource
    /// </exception>
    protected void AddModifierToBeAdded<TValue>(string modifierName, string fieldName,
        IModifier<TValue> modifier)
    {
        if (_modifiers.ContainsKey(modifierName))
            throw new ArgumentException(
                $"{GetClass()} does not contain modifier for name {modifierName}");

        AddingModifiers += values => values.AddModifierTo(fieldName, modifier);
        RemovingModifiers += values => values.RemoveModifierFrom(fieldName, modifier);
        _modifiers[modifierName] = modifier;
    }

    /// <summary>
    ///     Gets an IModifier of this ModifierResource.
    /// </summary>
    /// <param name="modifierName">The assigned name of the IModifier</param>
    /// <typeparam name="TModifier">The type of the modifier itself</typeparam>
    /// <returns>The modifier</returns>
    /// <exception cref="ArgumentException">
    ///     If the given fieldName wasn't assigned to an
    ///     IModifier, or if the IModifier assigned to fieldName is not a TModifier.
    /// </exception>
    protected TModifier GetModifier<TModifier>(string modifierName)
    {
        if (!_modifiers.ContainsKey(modifierName))
            throw new ArgumentException(
                $"{GetClass()} does not contain modifier for name {modifierName}");

        object untypedModifier = _modifiers[modifierName];
        if (untypedModifier is not TModifier modifier)
            throw new ArgumentException(
                $"{GetClass()}'s {modifierName} is not of type {typeof(TModifier)}");

        return modifier;
    }
}