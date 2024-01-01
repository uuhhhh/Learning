using Godot;
using Learning.Scripts.Entity.Physics.VelocitySources;

namespace Learning.Scripts.Entity.Physics.Intermediate; 

public partial class WallSnapping : Node {
    [Export] private bool Enabled { get; set; } = true;
    [Export] internal LeftRight Movement { get; private set; }
    [Export] public WallSnappingData SnapData { get; private set; }

    public bool IsWallSnapping {
        get => Enabled && _isWallSnapping;
        set {
            if (!Enabled || IsWallSnapping == value) return;
            _isWallSnapping = value;
            
            if (IsWallSnapping) {
                _wallSnapUsedUp = true;
                SnapData.AddModifiersTo(Movement.Air);
                WallSnapExpiry.Start();
                InWallSnapStartWindow = false;
                EmitSignal(SignalName.WallSnapStarted);
            } else {
                _wallSnapUsedUp = false;
                SnapData.RemoveModifiersFrom(Movement.Air);
                WallSnapExpiry.Stop();
                EmitSignal(SignalName.WallSnapStopped);
            }
        }
    }

    public bool InWallSnapStartWindow {
        get => Enabled && _inWallSnapStartWindow;
        set {
            if (!Enabled || InWallSnapStartWindow == value) return;
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
}