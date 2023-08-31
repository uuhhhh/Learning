using Godot;
using Learning.Scripts.Entity.Physics.VelocitySources;

namespace Learning.Scripts.Entity.Physics.Intermediate; 

public partial class WallSnapping : Node {
    [Export] internal LeftRight Movement { get; private set; }
    [Export] public WallSnappingData SnapData { get; private set; }
    
    private float AccelTimeMultiplier =>
        (float)Mathf.Lerp(SnapData.AccelTimeMultiplierInitial,
            SnapData.AccelTimeMultiplierFinal,
            WallSnapStartWindow.WaitTime != 0 ? WallSnapStartWindow.TimeLeft / WallSnapStartWindow.WaitTime : 0);

    public bool IsWallSnapping {
        get => _isWallSnapping;
        set {
            if (IsWallSnapping == value) return;
            _isWallSnapping = value;
            
            if (IsWallSnapping) {
                _wallSnapUsedUp = true;
                Movement.Air.AddModifier(_accelModifier);
                WallSnapExpiry.Start();
                InWallSnapStartWindow = false;
                EmitSignal(SignalName.WallSnapStarted);
            } else {
                _wallSnapUsedUp = false;
                Movement.Air.RemoveModifier(_accelModifier);
                WallSnapExpiry.Stop();
                EmitSignal(SignalName.WallSnapStopped);
            }
        }
    }

    public bool InWallSnapStartWindow {
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
    private bool _isWallSnapping;
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

        Movement.IntendedSpeedChange += _ => WallSnapCheck();
        WallSnapStartWindow.Timeout += () => InWallSnapStartWindow = false;
        WallSnapExpiry.Timeout += () => IsWallSnapping = false;

        _accelModifier = new WallSnapAccel(this);
    }
    
    public override void _ExitTree() {
        IsWallSnapping = false;
    }

    private void WallSnapCheck() {
        bool wasWallSnapping = IsWallSnapping;
        bool isWallSnapping = WallSnapStartWindow.TimeLeft > 0 
                              && Movement.IntendedSpeedScale != 0 
                              && Mathf.Sign(Movement.CurrentSpeedScale) == -Mathf.Sign(Movement.IntendedSpeedScale);

        IsWallSnapping = (wasWallSnapping, isWallSnapping, _wallSnapUsedUp) switch {
            (false, true, false) => true,
            (true, false, _) => false,
            _ => IsWallSnapping
        };
    }

    private class WallSnapAccel : IValueModifier {
        [Export] public int Priority { get; private set; }
        
        private readonly WallSnapping _snapping;

        public WallSnapAccel(WallSnapping snapping) {
            _snapping = snapping;
        }
        
        public TValue ApplyModifier<TValue>(string valueName, TValue value) {
            if (value is not float valueF) return value;
            
            float? result = valueName switch {
                nameof(LeftRightData.AccelBaseTime) => valueF * _snapping.AccelTimeMultiplier,
                nameof(LeftRightData.SpeedScaleHighDeltaPower) => _snapping.SnapData.SpeedScaleDeltaPowerReplacement,
                _ => null
            };
            
            return result.HasValue ? IValueModifier.Cast<float, TValue>(result.Value) : value;
        }
    }
}