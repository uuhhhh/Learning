using Godot;
using Learning.Scripts.Entity.Physics.VelocitySources;

namespace Learning.Scripts.Entity.Physics.Intermediate; 

[GlobalClass]
public partial class WallDraggingData : ResourceWithModifiers, IModifierGroup<FallingData> {
    [Export] public float UpwardsGravityScaleReplacement {
        get => GetValue<float>(nameof(UpwardsGravityScaleReplacement));
        private set => InitValue(nameof(UpwardsGravityScaleReplacement), value);
    }
    [Export] public float DownwardsGravityScaleReplacement {
        get => GetValue<float>(nameof(DownwardsGravityScaleReplacement));
        private set => InitValue(nameof(DownwardsGravityScaleReplacement), value);
    }
    [Export] public float MaxFallVelocityReplacement {
        get => GetValue<float>(nameof(MaxFallVelocityReplacement));
        private set => InitValue(nameof(MaxFallVelocityReplacement), value);
    }
    [Export] public float CeilingHitStopTimeScaleReplacement {
        get => GetValue<float>(nameof(CeilingHitStopTimeScaleReplacement));
        private set => InitValue(nameof(CeilingHitStopTimeScaleReplacement), value);
    }
    [Export] public float DecelToMaxVelocityTimePer100VelocityReplacement {
        get => GetValue<float>(nameof(DecelToMaxVelocityTimePer100VelocityReplacement));
        private set => InitValue(nameof(DecelToMaxVelocityTimePer100VelocityReplacement), value);
    }
    [Export] public float VelocityDragThreshold {
        get => GetValue<float>(nameof(VelocityDragThreshold));
        private set => InitValue(nameof(VelocityDragThreshold), value);
    }

    private FunctionalModifier<float> _upwardsGravityScaleReplacer;
    private FunctionalModifier<float> _downwardsGravityScaleReplacer;
    private FunctionalModifier<float> _maxVelocityReplacer;
    private FunctionalModifier<float> _ceilingHitStopTimeScaleReplacer;
    private FunctionalModifier<float> _decelToMaxVelocityTimePer100VelocityReplacer;

    public WallDraggingData() {
        _upwardsGravityScaleReplacer = new(
            _ => UpwardsGravityScaleReplacement, Modifier<object>.DefaultPriority, false);
        _downwardsGravityScaleReplacer = new(
            _ => DownwardsGravityScaleReplacement, Modifier<object>.DefaultPriority, false);
        _maxVelocityReplacer = new(
            _ => MaxFallVelocityReplacement, Modifier<object>.DefaultPriority, false);
        _ceilingHitStopTimeScaleReplacer = new(
            _ => CeilingHitStopTimeScaleReplacement, Modifier<object>.DefaultPriority, false);
        _decelToMaxVelocityTimePer100VelocityReplacer = new(
            _ => DecelToMaxVelocityTimePer100VelocityReplacement, Modifier<object>.DefaultPriority, false);
    }

    public void AddModifiersTo(FallingData values) {
        values.AddModifierTo(nameof(FallingData.UpwardsGravityScale), _upwardsGravityScaleReplacer);
        values.AddModifierTo(nameof(FallingData.DownwardsGravityScale), _downwardsGravityScaleReplacer);
        values.AddModifierTo(nameof(FallingData.MaxFallVelocity), _maxVelocityReplacer);
        values.AddModifierTo(nameof(FallingData.CeilingHitStopTimeScale), _ceilingHitStopTimeScaleReplacer);
        values.AddModifierTo(
            nameof(FallingData.DecelToMaxVelocityTimePer100Velocity), _decelToMaxVelocityTimePer100VelocityReplacer);
    }

    public void RemoveModifiersFrom(FallingData values) {
        values.RemoveModifierFrom(nameof(FallingData.UpwardsGravityScale), _upwardsGravityScaleReplacer);
        values.RemoveModifierFrom(nameof(FallingData.DownwardsGravityScale), _downwardsGravityScaleReplacer);
        values.RemoveModifierFrom(nameof(FallingData.MaxFallVelocity), _maxVelocityReplacer);
        values.RemoveModifierFrom(nameof(FallingData.CeilingHitStopTimeScale), _ceilingHitStopTimeScaleReplacer);
        values.RemoveModifierFrom(
            nameof(FallingData.DecelToMaxVelocityTimePer100Velocity), _decelToMaxVelocityTimePer100VelocityReplacer);
    }
}