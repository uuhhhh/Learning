namespace Learning.Scripts.Environment;

/// <summary>
/// An EnvObjectDetector mainly for detecting the Air part of EnvObjects.
/// </summary>
public partial class AirDetector : EnvObjectDetector
{
    /// <returns>The priority value for the given EnvObject's Air.</returns>
    protected override int GetPriorityOf(EnvObject envObject)
    {
        return envObject.Data.Air.Priority;
    }
}