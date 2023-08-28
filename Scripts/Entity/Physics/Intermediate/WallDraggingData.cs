using Godot;
using Learning.Scripts.Entity.Physics.VelocitySources;

namespace Learning.Scripts.Entity.Physics.Intermediate; 

[GlobalClass]
public partial class WallDraggingData : ResourceWithModifiers, IValueModifier {
    [Export] public float UpwardsGravityScaleReplacement {
        get => GetField<float>(nameof(UpwardsGravityScaleReplacement));
        private set => SetField(nameof(UpwardsGravityScaleReplacement), value);
    }
    [Export] public float DownwardsGravityScaleReplacement {
        get => GetField<float>(nameof(DownwardsGravityScaleReplacement));
        private set => SetField(nameof(DownwardsGravityScaleReplacement), value);
    }
    [Export] public float MaxVelocityReplacement {
        get => GetField<float>(nameof(MaxVelocityReplacement));
        private set => SetField(nameof(MaxVelocityReplacement), value);
    }
    [Export] public float CeilingHitStopTimeScaleReplacement {
        get => GetField<float>(nameof(CeilingHitStopTimeScaleReplacement));
        private set => SetField(nameof(CeilingHitStopTimeScaleReplacement), value);
    }
    [Export] public float DecelToMaxVelocityTimePer100VelocityReplacement {
        get => GetField<float>(nameof(DecelToMaxVelocityTimePer100VelocityReplacement));
        private set => SetField(nameof(DecelToMaxVelocityTimePer100VelocityReplacement), value);
    }
    
    [Export] public float VelocityDragThreshold {
        get => GetField<float>(nameof(VelocityDragThreshold));
        private set => SetField(nameof(VelocityDragThreshold), value);
    }
    
    [Export] public int Priority { get; private set; }

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

        return replacement.HasValue ? IValueModifier.Cast<float, TValue>(replacement.Value) : value;
    }
}