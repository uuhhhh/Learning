namespace Learning.Scripts.Values; 

public interface IValueWithModifiersGroup {
    public delegate void ModifiersUpdatedEventHandler();

    public event ModifiersUpdatedEventHandler ModifiersUpdated;
}