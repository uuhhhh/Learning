using Godot;
using Learning.Scripts.Values.Groups;
using Learning.Scripts.Values.Modifiers;

namespace Learning.Scripts.Entity.Physics.VelocitySources;

[GlobalClass]
public partial class FallingDataMultiplier : ModifierResource<FallingData>
{
    [Export]
    public FloatMultiplier UpwardsGravityScaleMultiplier
    {
        get => GetModifier<FloatMultiplier>(nameof(UpwardsGravityScaleMultiplier));
        private set => AddModifierToBeAdded(nameof(UpwardsGravityScaleMultiplier),
            nameof(FallingData.UpwardsGravityScale), value);
    }

    [Export]
    public FloatMultiplier DownwardsGravityScaleMultiplier
    {
        get => GetModifier<FloatMultiplier>(nameof(DownwardsGravityScaleMultiplier));
        private set => AddModifierToBeAdded(nameof(DownwardsGravityScaleMultiplier),
            nameof(FallingData.DownwardsGravityScale), value);
    }

    [Export]
    public FloatMultiplier MaxFallVelocityMultiplier
    {
        get => GetModifier<FloatMultiplier>(nameof(MaxFallVelocityMultiplier));
        private set => AddModifierToBeAdded(nameof(MaxFallVelocityMultiplier),
            nameof(FallingData.MaxFallVelocity), value);
    }

    [Export]
    public FloatMultiplier CeilingHitStopTimeScaleMultiplier
    {
        get => GetModifier<FloatMultiplier>(nameof(CeilingHitStopTimeScaleMultiplier));
        private set => AddModifierToBeAdded(nameof(CeilingHitStopTimeScaleMultiplier),
            nameof(FallingData.CeilingHitStopTimeScale), value);
    }

    [Export]
    public FloatMultiplier DecelToMaxVelocityTimePer100VelocityMultiplier
    {
        get => GetModifier<FloatMultiplier>(nameof(DecelToMaxVelocityTimePer100VelocityMultiplier));
        private set => AddModifierToBeAdded(nameof(DecelToMaxVelocityTimePer100VelocityMultiplier),
            nameof(FallingData.DecelToMaxVelocityTimePer100Velocity), value);
    }
}