namespace Learning.Scripts.Values.Modifiers;

/// <summary>
/// Values that indicate some value of priority for IModifiers.
/// The enum indexes are not the priority values these ModifierPriorities get mapped to.
/// </summary>
/// <see cref="ModifierPriorityExtensions.PriorityNum" />
public enum ModifierPriority
{
    DefaultPriority,
    WallDragging,
    WallSnapping
    
    // It is best to not change the enum indexes of currently-added ModifierPriorities,
    // since the Godot editor uses these indexes to indicate which ModifierPriority something is.
}

public static class ModifierPriorityExtensions
{
    /// <summary>
    /// Maps the given ModifierPriority to a priority value.
    /// </summary>
    /// <param name="priority">The ModifierPriority to get the priority value of</param>
    /// <returns>The priority value</returns>
    public static int PriorityNum(this ModifierPriority priority)
    {
        return priority switch
        {
            ModifierPriority.DefaultPriority => 0,
            ModifierPriority.WallDragging => -100,
            ModifierPriority.WallSnapping => -100,
            _ => 0
        };
    }
}