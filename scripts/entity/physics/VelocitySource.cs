using Godot;

namespace Learning.scripts.entity.physics; 

public abstract partial class VelocitySource : Node {
    [Export] public bool UseSpeedScale { get; private set; } = true;
    
    public Vector2 BaseVelocity { get; set; }
    
    public Vector2 Velocity => Enabled ? BaseVelocity * Multiplier : Vector2.Zero;
    
    public float Multiplier {
        get => _multiplier;
        set {
            _multiplierTween?.Kill();
            _multiplier = value;
        }
    }

    public bool Enabled { get; set; } = true;
    
    private Tween _baseVelocityXTween;
    private bool _baseVelocityXTweenReady;
    private Tween _baseVelocityYTween;
    private bool _baseVelocityYTweenReady;
    
    private float _multiplier = 1;
    private Tween _multiplierTween;
    private bool _multiplierTweenReady;

    public override void _PhysicsProcess(double delta) {
        if (!Enabled) return;
        
        BeginReadyTweens();
    }

    private void BeginReadyTweens() {
        if (_baseVelocityXTweenReady) {
            _baseVelocityXTween.Play();
            _baseVelocityXTweenReady = false;
        }
        if (_baseVelocityYTweenReady) {
            _baseVelocityYTween.Play();
            _baseVelocityYTweenReady = false;
        }
        if (_multiplierTweenReady) {
            _multiplierTween.Play();
            _multiplierTweenReady = false;
        }
    }

    protected (Tween, PropertyTweener) SmoothlySet(
        ref Tween toSet,
        string propertyName,
        Variant to,
        float duration) {
        toSet?.Kill();
        toSet = CreateTween();
        toSet.Pause();
        toSet.SetProcessMode(Tween.TweenProcessMode.Physics);
        if (UseSpeedScale) {
            toSet.SetSpeedScale(1 / duration);
        }

        PropertyTweener t
            = toSet.TweenProperty(this, propertyName, to, UseSpeedScale ? 1 : duration).FromCurrent();

        return (toSet, t);
    }
    
    protected (Tween, PropertyTweener) SmoothlySet(
        ref Tween toSet,
        string propertyName,
        Variant from,
        Variant to,
        float duration) {
        (Tween t, PropertyTweener p) = SmoothlySet(ref toSet, propertyName, to, duration);
        p = p.From(from);

        return (t, p);
    }

    public (Tween, PropertyTweener) SmoothlySetBaseVelocityX(float to, float duration) {
        _baseVelocityXTweenReady = true;
        return SmoothlySet(ref _baseVelocityXTween,
            $"{nameof(BaseVelocity)}:x", // have to hardcode "x" since "BaseVelocity:X" doesn't work
            to,
            duration);
    }

    public (Tween, PropertyTweener) SmoothlySetBaseVelocityX(float from, float to, float duration) {
        _baseVelocityXTweenReady = true;
        return SmoothlySet(ref _baseVelocityXTween,
            $"{nameof(BaseVelocity)}:x",
            from,
            to,
            duration);
    }

    public (Tween, PropertyTweener) SmoothlySetBaseVelocityY(float to, float duration) {
        _baseVelocityYTweenReady = true;
        return SmoothlySet(ref _baseVelocityYTween,
            $"{nameof(BaseVelocity)}:y",
            to,
            duration);
    }

    public (Tween, PropertyTweener) SmoothlySetBaseVelocityY(float from, float to, float duration) {
        _baseVelocityYTweenReady = true;
        return SmoothlySet(ref _baseVelocityYTween,
            $"{nameof(BaseVelocity)}:y",
            from,
            to,
            duration);
    }

    public (Tween, PropertyTweener) SmoothlySetMultiplier(float to, float duration) {
        _multiplierTweenReady = true;
        return SmoothlySet(ref _multiplierTween, nameof(_multiplier), to, duration);
    }

    public (Tween, PropertyTweener) SmoothlySetMultiplier(float from, float to, float duration) {
        _multiplierTweenReady = true;
        return SmoothlySet(ref _multiplierTween, nameof(_multiplier), from, to, duration);
    }

    public void KillAllTweens() {
        _baseVelocityXTween?.Kill();
        _baseVelocityXTweenReady = false;
        _baseVelocityYTween?.Kill();
        _baseVelocityYTweenReady = false;
        _multiplierTween?.Kill();
        _multiplierTweenReady = false;
    }
}