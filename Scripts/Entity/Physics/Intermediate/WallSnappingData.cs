using Godot;
using Learning.Scripts.Entity.Physics.VelocitySources;
using Learning.Scripts.Values.Groups;
using Learning.Scripts.Values.Modifiers;

namespace Learning.Scripts.Entity.Physics.Intermediate;

/// <summary>
///     Data used by a WallSnapping to determine the wall snapping movement. Modifies a LeftRightData
///     to modify its values to make the movement resemble a wall snap.
/// </summary>
[GlobalClass]
public partial class WallSnappingData : ResourceWithModifiers, IModifierGroup<LeftRightData>
{
    private FunctionalModifier<float> _accelTimeMultiplierModifier;

    public WallSnappingData()
    {
        _accelTimeMultiplierModifier = new FunctionalModifier<float>(
            value => AccelTimeMultiplier * value, AccelTimeMultiplierReplacementPriority, false);
    }

    /// <summary>
    ///     A scale factor for the LeftRightData's acceleration time,
    ///     when wall snapping at the beginning of the wall snap start window.
    /// </summary>
    [Export]
    public float AccelTimeMultiplierInitial
    {
        get => GetValue<float>(nameof(AccelTimeMultiplierInitial));
        private set => InitValue(nameof(AccelTimeMultiplierInitial), value);
    }

    /// <summary>
    ///     A scale factor for the LeftRightData's acceleration time,
    ///     when wall snapping at the end of the wall snap start window.
    /// </summary>
    [Export]
    public float AccelTimeMultiplierFinal
    {
        get => GetValue<float>(nameof(AccelTimeMultiplierFinal));
        private set => InitValue(nameof(AccelTimeMultiplierFinal), value);
    }

    /// <summary>
    ///     What to replace the LeftRightData's speed scale high delta power with.
    /// </summary>
    [Export]
    public ModifiedFloatReplacer SpeedScaleDeltaPowerReplacement
    {
        get => GetField<float, ModifiedFloatReplacer>(nameof(SpeedScaleDeltaPowerReplacement));
        private set => InitField(nameof(SpeedScaleDeltaPowerReplacement), value);
    }

    /// <summary>
    ///     The timer to use for determining the AccelTimeMultiplier.
    /// </summary>
    [Export]
    public Timer WallSnapStartWindow { get; private set; }

    /// <summary>
    ///     The modifier priority for the acceleration time multiplier.
    /// </summary>
    [Export]
    public ModifierPriority AccelTimeMultiplierReplacementPriority { get; private set; }

    /// <summary>
    ///     A value between AccelTimeInitial and AccelTimeFinal, depending on WallSnapTimeWindow's time.
    /// </summary>
    public float AccelTimeMultiplier =>
        (float)Mathf.Lerp(AccelTimeMultiplierInitial, AccelTimeMultiplierFinal,
            WallSnapTimeProportion);

    private double WallSnapTimeProportion => WallSnapStartWindow.WaitTime != 0
        ? WallSnapStartWindow.TimeLeft / WallSnapStartWindow.WaitTime
        : 0;

    public void AddModifiersTo(LeftRightData values)
    {
        values.AddModifierTo(nameof(LeftRightData.AccelBaseTime), _accelTimeMultiplierModifier);
        values.AddModifierTo(nameof(LeftRightData.SpeedScaleHighDeltaPower),
            SpeedScaleDeltaPowerReplacement);
    }

    public void RemoveModifiersFrom(LeftRightData values)
    {
        values.RemoveModifierFrom(nameof(LeftRightData.AccelBaseTime),
            _accelTimeMultiplierModifier);
        values.RemoveModifierFrom(nameof(LeftRightData.SpeedScaleHighDeltaPower),
            SpeedScaleDeltaPowerReplacement);
    }
}