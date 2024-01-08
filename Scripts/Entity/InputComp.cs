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

    private int _leftInputs;
    private int _rightInputs;

    public override void _Input(InputEvent input)
    {
        if (input.IsActionPressed("moveLeft"))
            InputLeft();
        else if (input.IsActionReleased("moveLeft"))
            StopInputLeft();
        else if (input.IsActionPressed("moveRight"))
            InputRight();
        else if (input.IsActionReleased("moveRight"))
            StopInputRight();
        else if (input.IsActionPressed("jump"))
            EmitSignal(SignalName.JumpInputOn);
        else if (input.IsActionReleased("jump")) EmitSignal(SignalName.JumpInputOff);
    }

    private void InputLeft()
    {
        _leftInputs++;

        if (_leftInputs == 1) EmitSignal(SignalName.LeftInputOn);
    }

    private void StopInputLeft()
    {
        _leftInputs--;

        if (_leftInputs == 0) EmitSignal(SignalName.LeftInputOff);
    }

    private void InputRight()
    {
        _rightInputs++;

        if (_rightInputs == 1) EmitSignal(SignalName.RightInputOn);
    }

    private void StopInputRight()
    {
        _rightInputs--;

        if (_rightInputs == 0) EmitSignal(SignalName.RightInputOff);
    }
}