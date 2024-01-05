namespace Learning.Scripts.Environment;

public partial class FloorDetector : EnvObjectDetector
{
    protected override int GetPriorityOf(EnvObject envObject)
    {
        return envObject.Data.FloorPriority;
    }
}