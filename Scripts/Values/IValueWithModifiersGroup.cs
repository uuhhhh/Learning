namespace Learning.Scripts.Values; 

public interface IValueWithModifiersGroup {
    public delegate void ModifiersUpdatedEventHandler();

    public event ModifiersUpdatedEventHandler ModifiersUpdated;

    public bool AddModifierTo<TValue>(string fieldName, IModifier<TValue> modifier);

    public bool RemoveModifierFrom<TValue>(string fieldName, IModifier<TValue> modifier);
}