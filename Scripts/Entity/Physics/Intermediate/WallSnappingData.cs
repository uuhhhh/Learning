using Godot;
using Learning.Scripts.Entity.Physics.VelocitySources;
using Learning.Scripts.Values.Groups;
using Learning.Scripts.Values.Modifiers;

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
    [Export] public ModifiedFloatReplacer SpeedScaleDeltaPowerReplacement {
        get => GetField<float, ModifiedFloatReplacer>(nameof(SpeedScaleDeltaPowerReplacement));
        private set => InitField(nameof(SpeedScaleDeltaPowerReplacement), value);
    }
    [Export] public Timer WallSnapStartWindow { get; private set; }
    [Export] public int AccelTimeMultiplierReplacementPriority { get; private set; }
    
    public float AccelTimeMultiplier =>
        (float)Mathf.Lerp(AccelTimeMultiplierInitial,
            AccelTimeMultiplierFinal,
            WallSnapStartWindow.WaitTime != 0 ? WallSnapStartWindow.TimeLeft / WallSnapStartWindow.WaitTime : 0);

    private FunctionalModifier<float> _accelTimeMultiplierModifier;
    
    public WallSnappingData() {
        _accelTimeMultiplierModifier = new(
            value => AccelTimeMultiplier * value, AccelTimeMultiplierReplacementPriority, false);
    }

    public void AddModifiersTo(LeftRightData values) {
        values.AddModifierTo(nameof(LeftRightData.AccelBaseTime), _accelTimeMultiplierModifier);
        values.AddModifierTo(nameof(LeftRightData.SpeedScaleHighDeltaPower), SpeedScaleDeltaPowerReplacement);
    }

    public void RemoveModifiersFrom(LeftRightData values) {
        values.RemoveModifierFrom(nameof(LeftRightData.AccelBaseTime), _accelTimeMultiplierModifier);
        values.RemoveModifierFrom(nameof(LeftRightData.SpeedScaleHighDeltaPower),
            SpeedScaleDeltaPowerReplacement);
    }
}