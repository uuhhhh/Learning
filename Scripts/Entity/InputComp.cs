using Godot;

namespace Learning.Scripts.Entity;

public partial class InputComp : Node {
	[Signal]
	public delegate void LeftInputOnEventHandler();

	[Signal]
	public delegate void LeftInputOffEventHandler();
	
	[Signal]
	public delegate void RightInputOnEventHandler();

	[Signal]
	public delegate void RightInputOffEventHandler();

	[Signal]
	public delegate void JumpInputOnEventHandler();

	[Signal]
	public delegate void JumpInputOffEventHandler();
	
	public override void _Input(InputEvent @event)
	{
		if (@event.IsActionPressed("moveLeft")) {
			EmitSignal(SignalName.LeftInputOn);
		} else if (@event.IsActionReleased("moveLeft")) {
			EmitSignal(SignalName.LeftInputOff);
		} else if (@event.IsActionPressed("moveRight")) {
			EmitSignal(SignalName.RightInputOn);
		} else if (@event.IsActionReleased("moveRight")) {
			EmitSignal(SignalName.RightInputOff);
		} else if (@event.IsActionPressed("jump")) {
			EmitSignal(SignalName.JumpInputOn);
		} else if (@event.IsActionReleased("jump")) {
			EmitSignal(SignalName.JumpInputOff);
		}
	}
}
