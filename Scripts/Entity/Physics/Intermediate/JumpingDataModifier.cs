using Godot;
using Learning.Scripts.Values.Groups;
using Learning.Scripts.Values.Modifiers;

namespace Learning.Scripts.Entity.Physics.Intermediate;

/// <summary>
/// A group of modifiers that can be applied to a JumpingData.
/// </summary>
[GlobalClass]
public partial class JumpingDataModifier : ModifierResource<JumpingData>
{
    /// <summary>
    /// A cap for the base number of jumps.
    /// </summary>
    [Export]
    public IntCapper NumJumpsCap
    {
        get => GetModifier<IntCapper>(nameof(NumJumpsCap));
        private set => AddModifierToBeAdded(nameof(NumJumpsCap),
            nameof(JumpingData.NumJumps), value);
    }

    /// <summary>
    /// A scale factor for the jump velocity.
    /// </summary>
    [Export]
    public VectorScaler VelocityMultiplier
    {
        get => GetModifier<VectorScaler>(nameof(VelocityMultiplier));
        private set => AddModifierToBeAdded(nameof(VelocityMultiplier),
            nameof(JumpingData.Velocity), value);
    }

    /// <summary>
    /// A scale factor for the jump x acceleration time.
    /// </summary>
    [Export]
    public FloatMultiplier AccelTimeXMultiplier
    {
        get => GetModifier<FloatMultiplier>(nameof(AccelTimeXMultiplier));
        private set => AddModifierToBeAdded(nameof(AccelTimeXMultiplier),
            nameof(JumpingData.AccelTimeX), value);
    }

    /// <summary>
    /// A scale factor for the jump y acceleration time.
    /// </summary>
    [Export]
    public FloatMultiplier AccelTimeYMultiplier
    {
        get => GetModifier<FloatMultiplier>(nameof(AccelTimeYMultiplier));
        private set => AddModifierToBeAdded(nameof(AccelTimeYMultiplier),
            nameof(JumpingData.AccelTimeY), value);
    }

    /// <summary>
    /// A scale factor for the jump cancel velocity.
    /// </summary>
    [Export]
    public FloatMultiplier CancelVelocityMultiplier
    {
        get => GetModifier<FloatMultiplier>(nameof(CancelVelocityMultiplier));
        private set => AddModifierToBeAdded(nameof(CancelVelocityMultiplier),
            nameof(JumpingData.CancelVelocity), value);
    }

    /// <summary>
    /// A scale factor for the jump cancel acceleration time.
    /// </summary>
    [Export]
    public FloatMultiplier CancelAccelTimeMultiplier
    {
        get => GetModifier<FloatMultiplier>(nameof(CancelAccelTimeMultiplier));
        private set => AddModifierToBeAdded(nameof(CancelAccelTimeMultiplier),
            nameof(JumpingData.CancelAccelTime), value);
    }
}