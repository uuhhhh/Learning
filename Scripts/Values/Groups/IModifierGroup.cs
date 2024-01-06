namespace Learning.Scripts.Values.Groups;

/// <summary>
///     A group of modifiers that can get added to the different IValueWithModifiers in an
///     IValueWithModifiersGroup. It is not necessarily the case that each IModifier in this group
///     gets added to each IValueWithModifier in the IValueWithModifiersGroup.
/// </summary>
/// <typeparam name="TValues">
///     The type of the group of IValueWithModifiers
///     that this IModifierGroup's modifiers concern.
/// </typeparam>
public interface IModifierGroup<in TValues> where TValues : IValueWithModifiersGroup
{
    /// <summary>
    ///     Adds this IModifierGroup's modifiers
    ///     to IValueWithModifiers in the given IValueWithModifierGroup.
    /// </summary>
    /// <param name="values">The group whose IValueWithModifiers to add modifiers to</param>
    public void AddModifiersTo(TValues values);

    /// <summary>
    ///     Removes this IModifierGroup's modifiers
    ///     from IValueWithModifiers in the given IValueWithModifierGroup.
    /// </summary>
    /// <param name="values">The group whose IValueWithModifiers to remove modifiers from</param>
    public void RemoveModifiersFrom(TValues values);
}