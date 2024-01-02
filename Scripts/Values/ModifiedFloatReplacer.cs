using Godot;

namespace Learning.Scripts.Values; 

[GlobalClass]
public partial class ModifiedFloatReplacer : ModifierWithModifiers<float> {
    [Export] public new float BaseValue {
        get => base.BaseValue;
        set => base.BaseValue = value;
    }

    public override float ApplyModifier(float value) {
        return ModifiedValue;
    }
}