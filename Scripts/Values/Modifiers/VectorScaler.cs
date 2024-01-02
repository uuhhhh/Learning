using Godot;

namespace Learning.Scripts.Values.Modifiers; 

[GlobalClass]
public partial class VectorScaler : Modifier<Vector2> {
    [Export] public Vector2 Scale { get; private set; }

    public override Vector2 ApplyModifier(Vector2 value) {
        return Scale * value;
    }
}