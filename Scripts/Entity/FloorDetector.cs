using Learning.Scripts.Environment;

namespace Learning.Scripts.Entity;

public partial class FloorDetector : EnvObjectDetector
{
    protected override int GetPriorityOf(EnvObject envObject)
    {
        return envObject.Data.FloorPriority;
    }
}