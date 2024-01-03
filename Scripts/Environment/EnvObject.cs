using Godot;

namespace Learning.Scripts.Environment;

public partial class EnvObject : Node2D
{
    [Export] public EnvObjectData Data { get; private set; }
}