namespace Learning.Scripts; 

public interface IValueWithModifiersGroup {
    public delegate void ModifiersUpdatedEventHandler();

    public event ModifiersUpdatedEventHandler ModifiersUpdated;
}