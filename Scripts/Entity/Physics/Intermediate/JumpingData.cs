using System;
using Godot;

namespace Learning.Scripts.Entity.Physics.Intermediate; 

[GlobalClass]
public partial class JumpingData : ResourceWithModifiers {
    public const int UnlimitedJumps = -1;
    
    [Export] public int NumJumps {
        get => GetValue<int>(nameof(NumJumps));
        private set => InitValue(nameof(NumJumps), value);
    }
    [Export] public Vector2 Velocity {
        get => GetValue<Vector2>(nameof(Velocity));
        private set => InitValue(nameof(Velocity), value);
    }
    [Export] public float AccelTimeX {
        get => GetValue<float>(nameof(AccelTimeX));
        private set => InitValue(nameof(AccelTimeX), value);
    }
    [Export] public float AccelTimeY {
        get => GetValue<float>(nameof(AccelTimeY));
        private set => InitValue(nameof(AccelTimeY), value);
    }
    [Export] public float CancelVelocity {
        get => GetValue<float>(nameof(CancelVelocity));
        private set => InitValue(nameof(CancelVelocity), value);
    }
    [Export] public float CancelAccelTime {
        get => GetValue<float>(nameof(CancelAccelTime));
        private set => InitValue(nameof(CancelAccelTime), value);
    }
    
    public static int MinNumJumps(int numJumps1, int numJumps2) {
        return (numJumps1, numJumps2) switch {
            (_, UnlimitedJumps) => numJumps1,
            (UnlimitedJumps, _) => numJumps2,
            (_, _) => Math.Min(numJumps1, numJumps2)
        };
    }
}