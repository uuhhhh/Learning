using Godot;

namespace Learning.Scripts.Entity.Physics.VelocitySources; 

[GlobalClass]
public partial class LeftRightData : ResourceWithModifiers {
    [Export] public float BaseSpeed {
        get => GetValue<float>(nameof(BaseSpeed));
        private set => InitValue(nameof(BaseSpeed), value);
    }
    [Export] public float AccelBaseTime {
        get => GetValue<float>(nameof(AccelBaseTime));
        private set => InitValue(nameof(AccelBaseTime), value);
    }
    [Export] public float DecelBaseTime {
        get => GetValue<float>(nameof(DecelBaseTime));
        private set => InitValue(nameof(DecelBaseTime), value);
    }
    [Export] public float SpeedScaleHighDeltaPower {
        get => GetValue<float>(nameof(SpeedScaleHighDeltaPower));
        private set => InitValue(nameof(SpeedScaleHighDeltaPower), value);
    }
}
