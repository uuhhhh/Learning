namespace Learning.Scripts.Environment;

/// <summary>
///     Values that indicate some value of priority for environment object data.
///     Used by EnvObjectDetector for determining which env object data is the "current" one.
///     The enum indexes are not the priority values these EnvironmentPriorities get mapped to.
/// </summary>
/// <see cref="EnvironmentPriorityExtensions.PriorityNum" />
public enum EnvironmentPriority
{
    DefaultPriority,
    LeastPriority

    // It is best to not change the enum indexes of currently-added EnvironmentPriorities,
    // since the Godot editor uses these indexes to indicate which EnvironmentPriority something is.
}

public static class EnvironmentPriorityExtensions
{
    /// <summary>
    ///     Maps the given EnvironmentPriority to a priority value.
    /// </summary>
    /// <param name="priority">The EnvironmentPriority to get the priority value of</param>
    /// <returns>The priority value</returns>
    public static int PriorityNum(this EnvironmentPriority priority)
    {
        return priority switch
        {
            EnvironmentPriority.DefaultPriority => 0,
            EnvironmentPriority.LeastPriority => int.MinValue,
            _ => int.MinValue
        };
    }
}