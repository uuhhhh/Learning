namespace Learning.Scripts.Environment;

/// <summary>
///     An EnvObjectDetector mainly for detecting the Wall part of EnvObjects.
/// </summary>
public partial class WallDetector : EnvObjectDetector
{
    /// <returns>The priority value for the given EnvObject's Wall.</returns>
    protected override EnvironmentPriority GetPriorityOf(EnvObject envObject)
    {
        return envObject.Data.Wall.Priority;
    }
}