using System;
using System.Collections.Generic;
using Godot;

namespace Learning.Scripts.Entity.Physics.Intermediate; 

[GlobalClass]
public partial class JumpingData : Resource, IValueModifierAggregate {
    public const int UnlimitedJumps = -1;
    
    [Export] public int NumJumps {
        get => ((IValueModifierAggregate)this).ApplyModifiers(nameof(NumJumps), _numJumps);
        private set => _numJumps = value;
    }
    [Export] public Vector2 Velocity {
        get => ((IValueModifierAggregate)this).ApplyModifiers(nameof(Velocity), _velocity);
        private set => _velocity = value;
    }
    [Export] public float AccelTimeX {
        get => ((IValueModifierAggregate)this).ApplyModifiers(nameof(AccelTimeX), _accelTimeX);
        private set => _accelTimeX = value;
    }
    [Export] public float AccelTimeY {
        get => ((IValueModifierAggregate)this).ApplyModifiers(nameof(AccelTimeY), _accelTimeY);
        private set => _accelTimeY = value;
    }
    [Export] public float CancelVelocity {
        get => ((IValueModifierAggregate)this).ApplyModifiers(nameof(CancelVelocity), _cancelVelocity);
        private set => _cancelVelocity = value;
    }
    [Export] public float CancelAccelTime {
        get => ((IValueModifierAggregate)this).ApplyModifiers(nameof(CancelAccelTime), _cancelAccelTime);
        private set => _cancelAccelTime = value;
    }

    public ISet<IValueModifier> Modifiers { get; } = IValueModifierAggregate.DefaultModifierInit();

    private int _numJumps;
    private Vector2 _velocity;
    private float _accelTimeX;
    private float _accelTimeY;
    private float _cancelVelocity;
    private float _cancelAccelTime;
    
    public static int MinNumJumps(int numJumps1, int numJumps2) {
        return (numJumps1, numJumps2) switch {
            (_, UnlimitedJumps) => numJumps1,
            (UnlimitedJumps, _) => numJumps2,
            (_, _) => Math.Min(numJumps1, numJumps2)
        };
    }
}