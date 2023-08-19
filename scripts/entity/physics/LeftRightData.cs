using Godot;

namespace Learning.scripts.entity.physics; 

[GlobalClass]
public partial class LeftRightData : Resource {
    [Export] public float BaseSpeed { get; private set; }
    [Export] public float AccelBaseTime { get; private set; }
    [Export] public float DecelBaseTime { get; private set; }
    [Export] public float SpeedScaleHighDeltaPower { get; private set; }
}