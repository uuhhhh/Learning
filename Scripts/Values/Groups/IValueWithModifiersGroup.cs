using Learning.Scripts.Values.Modifiers;

namespace Learning.Scripts.Values.Groups;

/// <summary>
/// A group of IValueWithModifiers, where modifiers can be added to them individually.
/// </summary>
public interface IValueWithModifiersGroup
{
    public delegate void ModifiersUpdatedEventHandler();
    /// <summary>
    /// An event that gets invoked when a modifier is added to an IValueWithModifiers
    /// of this IValueWithModifiersGroup.
    /// The event argument is the assigned name of the IValueWithModifiers that had a
    /// modifier added to it.
    /// </summary>

    public event ModifiersUpdatedEventHandler ModifiersUpdated;
    /// <summary>
    /// An event that gets invoked when a modifier is removed from an IValueWithModifiers
    /// of this IValueWithModifiersGroup.
    /// The event argument is the assigned name of the IValueWithModifiers that had a
    /// modifier removed from it.
    /// </summary>

    /// <summary>
    /// Adds a modifier to one of the IValueWithModifiers of this IValueWithModifiersGroup.
    /// </summary>
    /// <param name="fieldName">The assigned name of the IValueWithModifiers</param>
    /// <param name="modifier">The modifier to add</param>
    /// <typeparam name="TValue">The type of value that the IValueWithModifiers holds,
    /// and the type of value the modifier modifies</typeparam>
    /// <exception cref="ArgumentException">If the modifier type parameter and the
    /// IValueWithModifiers type parameter don't match.</exception>
    /// <returns>Whether the modifier was successfully added</returns>
    public bool AddModifierTo<TValue>(string fieldName, IModifier<TValue> modifier);

    /// <summary>
    /// Removes a modifier from one of the IValueWithModifiers of this IValueWithModifiersGroup.
    /// </summary>
    /// <param name="fieldName">The assigned name of the IValueWithModifiers</param>
    /// <param name="modifier">The modifier to remove</param>
    /// <typeparam name="TValue">The type of value that the IValueWithModifiers holds,
    /// and the type of value the modifier modifies</typeparam>
    /// <exception cref="ArgumentException">If the modifier type parameter and the
    /// IValueWithModifiers type parameter don't match.</exception>
    /// <returns>Whether the modifier was successfully removed</returns>
    public bool RemoveModifierFrom<TValue>(string fieldName, IModifier<TValue> modifier);
}