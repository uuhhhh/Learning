using Godot;

namespace Learning.Scripts.Entity.Physics.VelocitySources;

public partial class LeftRightDefaultPhys : DefaultPhys {
    [Export] private LeftRight ToLink { get; set; }
    [Export] private float WallSnapAccelTimeMultiplierInitial { get; set; }
    [Export] private float WallSnapAccelTimeMultiplierFinal { get; set; }
    [Export] private float WallSnapSpeedScaleDeltaPowerReplacement { get; set; }
    [Export] private int WallSnapModifierPriority { get; set; }
    
    private float WallSnapAccelTimeMultiplier =>
        (float)Mathf.Lerp(WallSnapAccelTimeMultiplierInitial,
            WallSnapAccelTimeMultiplierFinal,
            WallSnapStartWindow.WaitTime != 0 ? WallSnapStartWindow.TimeLeft / WallSnapStartWindow.WaitTime : 0);

    public bool WallSnapping {
        get => _wallSnapping;
        set {
            if (WallSnapping == value) return;
            _wallSnapping = value;
            
            if (WallSnapping) {
                _wallSnapUsedUp = true;
                ToLink.Air.AddModifier(_accelModifier);
                WallSnapExpiry.Start();
                InWallSnapStartWindow = false;
                EmitSignal(SignalName.WallSnapStarted);
            } else {
                _wallSnapUsedUp = false;
                ToLink.Air.RemoveModifier(_accelModifier);
                WallSnapExpiry.Stop();
                EmitSignal(SignalName.WallSnapStopped);
            }
        }
    }

    private bool InWallSnapStartWindow {
        get => _inWallSnapStartWindow;
        set {
            if (InWallSnapStartWindow == value) return;
            _inWallSnapStartWindow = value;

            if (InWallSnapStartWindow) {
                WallSnapStartWindow.Start();
                _wallSnapUsedUp = false;
            } else {
                WallSnapStartWindow.Stop();
                _wallSnapUsedUp = false;
            }
        }
    }

    private Timer WallSnapStartWindow { get; set; }
    private Timer WallSnapExpiry { get; set; }
    private bool _wallSnapping;
    private bool _inWallSnapStartWindow;
    private bool _wallSnapUsedUp;

    private WallSnapAccel _accelModifier;
    
    [Signal]
    public delegate void WallSnapStartedEventHandler();
    [Signal]
    public delegate void WallSnapStoppedEventHandler();
    
    public override void _Ready() {
        WallSnapStartWindow = GetNode<Timer>(nameof(WallSnapStartWindow));
        WallSnapExpiry = GetNode<Timer>(nameof(WallSnapExpiry));

        ToLink.IntendedSpeedChange += _ => WallSnapCheck();
        WallSnapStartWindow.Timeout += () => InWallSnapStartWindow = false;
        WallSnapExpiry.Timeout += () => WallSnapping = false;

        _accelModifier = new WallSnapAccel(this);
    }

    public override void _ExitTree() {
        WallSnapping = false;
    }

    internal override void OnBecomeOnFloor(KinematicComp physics) {
        ToLink.IsOnGround = true;
    }

    internal override void OnBecomeOffFloor(KinematicComp physics) {
        ToLink.IsOnGround = false;

        if (physics.Velocity.Y >= 0) {
            InWallSnapStartWindow = true;
        }
    }

    private void WallSnapCheck() {
        bool wasWallSnapping = WallSnapping;
        bool isWallSnapping = WallSnapStartWindow.TimeLeft > 0 
                                && ToLink.IntendedSpeedScale != 0 
                                && Mathf.Sign(ToLink.CurrentSpeedScale) == -Mathf.Sign(ToLink.IntendedSpeedScale);

        switch (wasWallSnapping, isWallSnapping, _wallSnapUsedUp) {
            case (false, true, false):
                WallSnapping = true;
                break;
            case (true, false, _):
                WallSnapping = false;
                break;
        }
    }

    private class WallSnapAccel : IValueModifier {
        public int Priority { get; }

        private readonly LeftRightDefaultPhys _phys;

        public WallSnapAccel(LeftRightDefaultPhys phys) {
            _phys = phys;
            Priority = phys.WallSnapModifierPriority;
        }
        
        public TValue ApplyModifier<TValue>(string valueName, TValue value) {
            if (value is not float valueF) return value;
            
            float? result = valueName switch {
                nameof(LeftRightData.AccelBaseTime) => valueF * _phys.WallSnapAccelTimeMultiplier,
                nameof(LeftRightData.SpeedScaleHighDeltaPower) => _phys.WallSnapSpeedScaleDeltaPowerReplacement,
                _ => null
            };
            
            return result.HasValue ? IValueModifier.Cast<float, TValue>(result.Value) : value;
        }
    }
}