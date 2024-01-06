using Godot;

namespace Learning.Scripts.Entity;

/// <summary>
///     A collection of signals that are emitted when the user performs certain inputs.
/// </summary>
public partial class InputComp : Node
{
    /// <summary>
    ///     A signal for when the user stops inputting the "jump" input.
    /// </summary>
    [Signal]
    public delegate void JumpInputOffEventHandler();

    /// <summary>
    ///     A signal for when the user starts inputting the "jump" input.
    /// </summary>
    [Signal]
    public delegate void JumpInputOnEventHandler();

    /// <summary>
    ///     A signal for when the user stops inputting the "left" input.
    /// </summary>
    [Signal]
    public delegate void LeftInputOffEventHandler();

    /// <summary>
    ///     A signal for when the user starts inputting the "left" input.
    /// </summary>
    [Signal]
    public delegate void LeftInputOnEventHandler();

    /// <summary>
    ///     A signal for when the user stops inputting the "right" input.
    /// </summary>
    [Signal]
    public delegate void RightInputOffEventHandler();

    /// <summary>
    ///     A signal for when the user starts inputting the "right" input.
    /// </summary>
    [Signal]
    public delegate void RightInputOnEventHandler();

    public override void _Input(InputEvent input)
    {
        if (input.IsActionPressed("moveLeft"))
            EmitSignal(SignalName.LeftInputOn);
        else if (input.IsActionReleased("moveLeft"))
            EmitSignal(SignalName.LeftInputOff);
        else if (input.IsActionPressed("moveRight"))
            EmitSignal(SignalName.RightInputOn);
        else if (input.IsActionReleased("moveRight"))
            EmitSignal(SignalName.RightInputOff);
        else if (input.IsActionPressed("jump"))
            EmitSignal(SignalName.JumpInputOn);
        else if (input.IsActionReleased("jump")) EmitSignal(SignalName.JumpInputOff);
    }
}