using Godot;
using Learning.Scripts.Entity.Physics.VelocitySources;
using Learning.Scripts.Values;

namespace Learning.Scripts.Entity.Physics.Intermediate; 

[GlobalClass]
public partial class WallSnappingData : ResourceWithModifiers, IModifierGroup<LeftRightData> {
    [Export] public float AccelTimeMultiplierInitial {
        get => GetValue<float>(nameof(AccelTimeMultiplierInitial));
        private set => InitValue(nameof(AccelTimeMultiplierInitial), value);
    }
    [Export] public float AccelTimeMultiplierFinal {
        get => GetValue<float>(nameof(AccelTimeMultiplierFinal));
        private set => InitValue(nameof(AccelTimeMultiplierFinal), value);
    }
    [Export] public float SpeedScaleDeltaPowerReplacement {
        get => GetValue<float>(nameof(SpeedScaleDeltaPowerReplacement));
        private set => InitValue(nameof(SpeedScaleDeltaPowerReplacement), value);
    }
    [Export] public Timer WallSnapStartWindow { get; private set; }
    
    public float AccelTimeMultiplier =>
        (float)Mathf.Lerp(AccelTimeMultiplierInitial,
            AccelTimeMultiplierFinal,
            WallSnapStartWindow.WaitTime != 0 ? WallSnapStartWindow.TimeLeft / WallSnapStartWindow.WaitTime : 0);

    private FunctionalModifier<float> _accelTimeMultiplierModifier;
    private FunctionalModifier<float> _speedScaleDeltaPowerReplacementModifier;
    
    public WallSnappingData() {
        _accelTimeMultiplierModifier = new(
            value => AccelTimeMultiplier * value, Modifier<object>.DefaultPriority, false);
        
        _speedScaleDeltaPowerReplacementModifier = new(
            _ => SpeedScaleDeltaPowerReplacement, Modifier<object>.DefaultPriority, false);
    }

    public void AddModifiersTo(LeftRightData values) {
        values.AddModifierTo(nameof(LeftRightData.AccelBaseTime), _accelTimeMultiplierModifier);
        values.AddModifierTo(nameof(LeftRightData.SpeedScaleHighDeltaPower), _speedScaleDeltaPowerReplacementModifier);
    }

    public void RemoveModifiersFrom(LeftRightData values) {
        values.RemoveModifierFrom(nameof(LeftRightData.AccelBaseTime), _accelTimeMultiplierModifier);
        values.RemoveModifierFrom(nameof(LeftRightData.SpeedScaleHighDeltaPower),
            _speedScaleDeltaPowerReplacementModifier);
    }
}