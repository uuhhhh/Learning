using Godot;
using Learning.Scripts.Environment;

namespace Learning.Scripts.Entity; 

public partial class WallDetector : EnvObjectDetector {
    public override void _Ready() {
        base._Ready();

        EnvObjectEnteredArea += eo => GD.Print($"wall entered: {eo}");
        EnvObjectExitedArea += eo => GD.Print($"wall exited: {eo}");
        NewHighestPriorityEnvObject += eo => GD.Print($"new highest prio: {eo}");
        ZeroToOneEnvObjects += () => GD.Print("zero to one walls");
        ZeroEnvObjects += () => GD.Print("zero walls");
    }
    
    protected override int GetPriorityOf(EnvObject envObject) {
        return envObject.WallPriority;
    }
}