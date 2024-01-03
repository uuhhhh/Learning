using Godot;

namespace Learning.Scripts.Values.Modifiers;

[GlobalClass]
public partial class FloatMultiplier : Modifier<float>
{
    [Export] public float Multiplier { get; private set; }

    public override float ApplyModifier(float value)
    {
        return value * Multiplier;
    }
}