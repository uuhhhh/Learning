using Godot;

namespace Learning.Scripts.Entity.Physics.VelocitySources; 

[GlobalClass]
public partial class LeftRightData : ResourceWithModifiers {
    [Export] public float BaseSpeed {
        get => GetField<float>(nameof(BaseSpeed));
        private set => SetField(nameof(BaseSpeed), value);
    }
    [Export] public float AccelBaseTime {
        get => GetField<float>(nameof(AccelBaseTime));
        private set => SetField(nameof(AccelBaseTime), value);
    }
    [Export] public float DecelBaseTime {
        get => GetField<float>(nameof(DecelBaseTime));
        private set => SetField(nameof(DecelBaseTime), value);
    }
    [Export] public float SpeedScaleHighDeltaPower {
        get => GetField<float>(nameof(SpeedScaleHighDeltaPower));
        private set => SetField(nameof(SpeedScaleHighDeltaPower), value);
    }
}
