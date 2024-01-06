namespace Learning.Scripts.Environment;

/// <summary>
/// An EnvObjectDetector mainly for detecting the Floor part of EnvObjects.
/// </summary>
public partial class FloorDetector : EnvObjectDetector
{
    /// <returns>The priority value for the given EnvObject's Floor.</returns>
    protected override int GetPriorityOf(EnvObject envObject)
    {
        return envObject.Data.Floor.Priority;
    }
}