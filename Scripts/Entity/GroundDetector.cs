using Godot;
using Learning.Scripts.Environment;

namespace Learning.Scripts.Entity; 

public partial class GroundDetector : EnvObjectDetector {
    protected override int GetPriorityOf(EnvObject envObject) {
        return envObject.GroundPriority;
    }
}