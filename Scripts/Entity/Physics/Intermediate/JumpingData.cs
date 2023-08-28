using System;
using Godot;

namespace Learning.Scripts.Entity.Physics.Intermediate; 

[GlobalClass]
public partial class JumpingData : ResourceWithModifiers {
    public const int UnlimitedJumps = -1;
    
    [Export] public int NumJumps {
        get => GetField<int>(nameof(NumJumps));
        private set => SetField(nameof(NumJumps), value);
    }
    [Export] public Vector2 Velocity {
        get => GetField<Vector2>(nameof(Velocity));
        private set => SetField(nameof(Velocity), value);
    }
    [Export] public float AccelTimeX {
        get => GetField<float>(nameof(AccelTimeX));
        private set => SetField(nameof(AccelTimeX), value);
    }
    [Export] public float AccelTimeY {
        get => GetField<float>(nameof(AccelTimeY));
        private set => SetField(nameof(AccelTimeY), value);
    }
    [Export] public float CancelVelocity {
        get => GetField<float>(nameof(CancelVelocity));
        private set => SetField(nameof(CancelVelocity), value);
    }
    [Export] public float CancelAccelTime {
        get => GetField<float>(nameof(CancelAccelTime));
        private set => SetField(nameof(CancelAccelTime), value);
    }
    
    public static int MinNumJumps(int numJumps1, int numJumps2) {
        return (numJumps1, numJumps2) switch {
            (_, UnlimitedJumps) => numJumps1,
            (UnlimitedJumps, _) => numJumps2,
            (_, _) => Math.Min(numJumps1, numJumps2)
        };
    }
}