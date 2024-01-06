using Godot;
using Learning.Scripts.Values.Groups;
using Learning.Scripts.Values.Modifiers;

namespace Learning.Scripts.Entity.Physics.VelocitySources;

/// <summary>
///     A group of modifiers that can be applied to a LeftRightData.
/// </summary>
[GlobalClass]
public partial class LeftRightDataMultiplier : ModifierResource<LeftRightData>
{
    /// <summary>
    ///     A scale factor for LeftRightData's BaseSpeed.
    /// </summary>
    [Export]
    public FloatMultiplier BaseSpeedMultiplier
    {
        get => GetModifier<FloatMultiplier>(nameof(BaseSpeedMultiplier));
        private set => AddModifierToBeAdded(nameof(BaseSpeedMultiplier),
            nameof(LeftRightData.BaseSpeed), value);
    }

    /// <summary>
    ///     A scale factor for LeftRightData's BaseAccelTime.
    /// </summary>
    [Export]
    public FloatMultiplier AccelBaseTimeMultiplier
    {
        get => GetModifier<FloatMultiplier>(nameof(AccelBaseTimeMultiplier));
        private set => AddModifierToBeAdded(nameof(AccelBaseTimeMultiplier),
            nameof(LeftRightData.AccelBaseTime), value);
    }

    /// <summary>
    ///     A scale factor for LeftRightData's DecelBaseTime.
    /// </summary>
    [Export]
    public FloatMultiplier DecelBaseTimeMultiplier
    {
        get => GetModifier<FloatMultiplier>(nameof(DecelBaseTimeMultiplier));
        private set => AddModifierToBeAdded(nameof(DecelBaseTimeMultiplier),
            nameof(LeftRightData.DecelBaseTime), value);
    }

    /// <summary>
    ///     A scale factor for LeftRightData's SpeedScaleHighDeltaPower.
    /// </summary>
    [Export]
    public FloatMultiplier SpeedScaleHighDeltaPowerMultiplier
    {
        get => GetModifier<FloatMultiplier>(nameof(SpeedScaleHighDeltaPowerMultiplier));
        private set => AddModifierToBeAdded(nameof(SpeedScaleHighDeltaPowerMultiplier),
            nameof(LeftRightData.SpeedScaleHighDeltaPower), value);
    }
}