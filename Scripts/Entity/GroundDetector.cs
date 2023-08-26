using Godot;
using Learning.Scripts.Environment;

namespace Learning.Scripts.Entity; 

public partial class GroundDetector : EnvObjectDetector {
    public override void _Ready() {
        base._Ready();

        EnvObjectEnteredArea += eo => GD.Print($"ground entered: {eo}");
        EnvObjectExitedArea += eo => GD.Print($"ground exited: {eo}");
        NewHighestPriorityEnvObject += eo => GD.Print($"new highest prio: {eo}");
        ZeroToOneEnvObjects += () => GD.Print("zero to one grounds");
        ZeroEnvObjects += () => GD.Print("zero grounds");
    }
    protected override int GetPriorityOf(EnvObject envObject) {
        return envObject.GroundPriority;
    }
}