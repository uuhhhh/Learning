using Godot;

namespace Learning.Scripts.Environment; 

public partial class EnvObject : Node2D {
    [Export] public int GroundPriority { get; private set; }
    [Export] public int WallPriority { get; private set; }
}