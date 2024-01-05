namespace Learning.Scripts.Environment;

public partial class WallDetector : EnvObjectDetector
{
    protected override int GetPriorityOf(EnvObject envObject)
    {
        return envObject.Data.WallPriority;
    }
}