using System.Collections.Generic;
using Godot;
using Learning.Scripts.Entity.Physics.VelocitySources;

namespace Learning.Scripts.Entity.Physics.Intermediate; 

[GlobalClass]
public partial class WallDraggingData : Resource, IValueModifier, IValueModifierAggregate {
    [Export] public float UpwardsGravityScaleReplacement {
        get => ((IValueModifierAggregate)this).ApplyModifiers(
            nameof(UpwardsGravityScaleReplacement), _upwardsGravityScaleReplacement);
        private set => _upwardsGravityScaleReplacement = value;
    }
    [Export] public float DownwardsGravityScaleReplacement {
        get => ((IValueModifierAggregate)this).ApplyModifiers(
            nameof(DownwardsGravityScaleReplacement), _downwardsGravityScaleReplacement);
        private set => _downwardsGravityScaleReplacement = value;
    }
    [Export] public float MaxVelocityReplacement {
        get => ((IValueModifierAggregate)this).ApplyModifiers(
            nameof(MaxVelocityReplacement), _maxVelocityReplacement);
        private set => _maxVelocityReplacement = value;
    }
    [Export] public float CeilingHitStopTimeScaleReplacement {
        get => ((IValueModifierAggregate)this).ApplyModifiers(
            nameof(CeilingHitStopTimeScaleReplacement), _ceilingHitStopTimeScaleReplacement);
        private set => _ceilingHitStopTimeScaleReplacement = value;
    }
    [Export] public float DecelToMaxVelocityTimePer100VelocityReplacement {
        get => ((IValueModifierAggregate)this).ApplyModifiers(
            nameof(DecelToMaxVelocityTimePer100VelocityReplacement), _decelToMaxVelocityTimePer100VelocityReplacement);
        private set => _decelToMaxVelocityTimePer100VelocityReplacement = value;
    }
    
    [Export] public float VelocityDragThreshold {
        get => ((IValueModifierAggregate)this).ApplyModifiers(
            nameof(VelocityDragThreshold), _velocityDragThreshold);
        private set => _velocityDragThreshold = value;
    }
    
    [Export] public int Priority { get; private set; }

    public ISet<IValueModifier> Modifiers { get; } = IValueModifierAggregate.DefaultModifierInit();

    private float _upwardsGravityScaleReplacement;
    private float _downwardsGravityScaleReplacement;
    private float _maxVelocityReplacement;
    private float _ceilingHitStopTimeScaleReplacement;
    private float _decelToMaxVelocityTimePer100VelocityReplacement;

    private float _velocityDragThreshold;
    
    public TValue ApplyModifier<TValue>(string valueName, TValue value) {
        if (value is not float) return value;
        
        float? replacement = valueName switch {
            nameof(FallingData.UpwardsGravityScale) => UpwardsGravityScaleReplacement,
            nameof(FallingData.DownwardsGravityScale) => DownwardsGravityScaleReplacement,
            nameof(FallingData.MaxVelocity) => MaxVelocityReplacement,
            nameof(FallingData.CeilingHitStopTimeScale) => CeilingHitStopTimeScaleReplacement,
            nameof(FallingData.DecelToMaxVelocityTimePer100Velocity) => DecelToMaxVelocityTimePer100VelocityReplacement,
            _ => null
        };

        return replacement is TValue replacementT ? replacementT : value;
    }
}