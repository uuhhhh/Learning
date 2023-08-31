using Godot;

namespace Learning.Scripts.Entity.Physics.Intermediate; 

[GlobalClass]
public partial class WallSnappingData : ResourceWithModifiers {
    [Export] public float AccelTimeMultiplierInitial {
        get => GetField<float>(nameof(AccelTimeMultiplierInitial));
        private set => SetField(nameof(AccelTimeMultiplierInitial), value);
    }
    [Export] public float AccelTimeMultiplierFinal {
        get => GetField<float>(nameof(AccelTimeMultiplierFinal));
        private set => SetField(nameof(AccelTimeMultiplierFinal), value);
    }
    [Export] public float SpeedScaleDeltaPowerReplacement {
        get => GetField<float>(nameof(SpeedScaleDeltaPowerReplacement));
        private set => SetField(nameof(SpeedScaleDeltaPowerReplacement), value);
    }
    
    protected override void RefreshAllFields() {
        RefreshField<float>(nameof(AccelTimeMultiplierInitial));
        RefreshField<float>(nameof(AccelTimeMultiplierFinal));
        RefreshField<float>(nameof(SpeedScaleDeltaPowerReplacement));
    }
}