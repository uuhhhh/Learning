using System.Collections.Generic;
using Godot;

namespace Learning.Scripts.Entity.Physics.VelocitySources; 

[GlobalClass]
public partial class FallingData : Resource, IValueModifierAggregate {
    [Export] public float UpwardsGravityScale {
        get => ((IValueModifierAggregate)this).ApplyModifiers(nameof(UpwardsGravityScale), _upwardsGravityScale);
        private set => _upwardsGravityScale = value;
    }
    [Export] public float DownwardsGravityScale {
        get => ((IValueModifierAggregate)this).ApplyModifiers(nameof(DownwardsGravityScale), _downwardsGravityScale);
        private set => _downwardsGravityScale = value;
    }
    [Export] public float MaxVelocity {
        get => ((IValueModifierAggregate)this).ApplyModifiers(nameof(MaxVelocity), _maxVelocity);
        private set => _maxVelocity = value;
    }
    [Export] public float CeilingHitStopTimeScale {
        get => ((IValueModifierAggregate)this)
            .ApplyModifiers(nameof(CeilingHitStopTimeScale), _ceilingHitStopTimeScale);
        private set => _ceilingHitStopTimeScale = value;
    }
    [Export] public float DecelToMaxVelocityTimePer100Velocity {
        get => ((IValueModifierAggregate)this)
            .ApplyModifiers(nameof(DecelToMaxVelocityTimePer100Velocity), _decelToMaxVelocityTimePer100Velocity);
        private set => _decelToMaxVelocityTimePer100Velocity = value;
    }

    public float DecelToMaxVelocityTimePerVelocity => DecelToMaxVelocityTimePer100Velocity / 100;

    public ISet<IValueModifier> Modifiers { get; } = IValueModifierAggregate.DefaultModifierInit();

    private float _upwardsGravityScale;
    private float _downwardsGravityScale;
    private float _maxVelocity;
    private float _ceilingHitStopTimeScale;
    private float _decelToMaxVelocityTimePer100Velocity;
}