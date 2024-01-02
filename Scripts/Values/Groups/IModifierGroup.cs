namespace Learning.Scripts.Values.Groups; 

public interface IModifierGroup<in TValues> where TValues : IValueWithModifiersGroup {
    public void AddModifiersTo(TValues values);

    public void RemoveModifiersFrom(TValues values);
}