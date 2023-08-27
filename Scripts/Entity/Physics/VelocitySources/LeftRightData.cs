using System.Collections.Generic;
using Godot;

namespace Learning.Scripts.Entity.Physics.VelocitySources; 

[GlobalClass]
public partial class LeftRightData : Resource, IValueModifierAggregate {
    [Export] public float BaseSpeed {
        get => ((IValueModifierAggregate)this).ApplyModifiers(nameof(BaseSpeed), _baseSpeed);
        private set => _baseSpeed = value;
    }
    [Export] public float AccelBaseTime {
        get => ((IValueModifierAggregate)this).ApplyModifiers(nameof(AccelBaseTime), _accelBaseTime);
        private set => _accelBaseTime = value;
    }
    [Export] public float DecelBaseTime {
        get => ((IValueModifierAggregate)this).ApplyModifiers(nameof(DecelBaseTime), _decelBaseTime);
        private set => _decelBaseTime = value;
    }
    [Export] public float SpeedScaleHighDeltaPower {
        get => ((IValueModifierAggregate)this)
            .ApplyModifiers(nameof(SpeedScaleHighDeltaPower), _speedScaleHighDeltaPower);
        private set => _speedScaleHighDeltaPower = value;
    }

    public ISet<IValueModifier> Modifiers { get; } = IValueModifierAggregate.DefaultModifierInit();

    private float _baseSpeed;
    private float _accelBaseTime;
    private float _decelBaseTime;
    private float _speedScaleHighDeltaPower;
}
