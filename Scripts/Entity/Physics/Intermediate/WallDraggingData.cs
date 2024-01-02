using Godot;
using Learning.Scripts.Entity.Physics.VelocitySources;
using Learning.Scripts.Values;

namespace Learning.Scripts.Entity.Physics.Intermediate; 

[GlobalClass]
public partial class WallDraggingData : ResourceWithModifiers, IModifierGroup<FallingData> {
    [Export] public ModifiedFloatReplacer UpwardsGravityScaleReplacement {
        get => GetField<float, ModifiedFloatReplacer>(nameof(UpwardsGravityScaleReplacement));
        private set => InitField(nameof(UpwardsGravityScaleReplacement), value);
    }
    [Export] public ModifiedFloatReplacer DownwardsGravityScaleReplacement {
        get => GetField<float, ModifiedFloatReplacer>(nameof(DownwardsGravityScaleReplacement));
        private set => InitField(nameof(DownwardsGravityScaleReplacement), value);
    }
    [Export] public ModifiedFloatReplacer MaxFallVelocityReplacement {
        get => GetField<float, ModifiedFloatReplacer>(nameof(MaxFallVelocityReplacement));
        private set => InitField(nameof(MaxFallVelocityReplacement), value);
    }
    [Export] public ModifiedFloatReplacer CeilingHitStopTimeScaleReplacement {
        get => GetField<float, ModifiedFloatReplacer>(nameof(CeilingHitStopTimeScaleReplacement));
        private set => InitField(nameof(CeilingHitStopTimeScaleReplacement), value);
    }
    [Export] public ModifiedFloatReplacer DecelToMaxVelocityTimePer100VelocityReplacement {
        get => GetField<float, ModifiedFloatReplacer>(nameof(DecelToMaxVelocityTimePer100VelocityReplacement));
        private set => InitField(nameof(DecelToMaxVelocityTimePer100VelocityReplacement), value);
    }
    [Export] public float VelocityDragThreshold {
        get => GetValue<float>(nameof(VelocityDragThreshold));
        private set => InitValue(nameof(VelocityDragThreshold), value);
    }

    public void AddModifiersTo(FallingData values) {
        values.AddModifierTo(nameof(FallingData.UpwardsGravityScale), UpwardsGravityScaleReplacement);
        values.AddModifierTo(nameof(FallingData.DownwardsGravityScale), DownwardsGravityScaleReplacement);
        values.AddModifierTo(nameof(FallingData.MaxFallVelocity), MaxFallVelocityReplacement);
        values.AddModifierTo(nameof(FallingData.CeilingHitStopTimeScale), CeilingHitStopTimeScaleReplacement);
        values.AddModifierTo(
            nameof(FallingData.DecelToMaxVelocityTimePer100Velocity), DecelToMaxVelocityTimePer100VelocityReplacement);
    }

    public void RemoveModifiersFrom(FallingData values) {
        values.RemoveModifierFrom(nameof(FallingData.UpwardsGravityScale), UpwardsGravityScaleReplacement);
        values.RemoveModifierFrom(nameof(FallingData.DownwardsGravityScale), DownwardsGravityScaleReplacement);
        values.RemoveModifierFrom(nameof(FallingData.MaxFallVelocity), MaxFallVelocityReplacement);
        values.RemoveModifierFrom(nameof(FallingData.CeilingHitStopTimeScale), CeilingHitStopTimeScaleReplacement);
        values.RemoveModifierFrom(
            nameof(FallingData.DecelToMaxVelocityTimePer100Velocity), DecelToMaxVelocityTimePer100VelocityReplacement);
    }
}