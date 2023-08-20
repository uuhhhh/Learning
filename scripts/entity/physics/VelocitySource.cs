using Godot;

namespace Learning.scripts.entity.physics; 

public abstract partial class VelocitySource : Node {
    [Export] public bool ExcludeThisVelocity { get; private set; }
    [Export] public bool UseSpeedScale { get; private set; } = true;
    [Export] public float SmoothlyDisableTime { get; private set; } = .25f;
    [Export] public float SmoothlyEnableTime { get; private set; } = .25f;

    public bool Enabled { get; set; } = true;

    public float Multiplier { get; set; } = 1;
    
    public Vector2 BaseVelocity { get; set; }

    public Vector2 BaseVelocityAfterTransition =>
        TransitioningBaseVelocityX || TransitioningVelocityY ?
            new Vector2(
                TransitioningBaseVelocityX ? _tweeningBaseVelocityXTo : BaseVelocity.X,
                TransitioningVelocityY ? _tweeningBaseVelocityYTo : BaseVelocity.Y)
            : BaseVelocity;
    
    public Vector2 Velocity => Enabled ? BaseVelocity * Multiplier : Vector2.Zero;

    public Vector2 VelocityAfterTransition => Enabled ? BaseVelocityAfterTransition * Multiplier : Vector2.Zero;
    
    public bool TransitioningBaseVelocityX => _baseVelocityXTween != null && _baseVelocityXTween.IsValid();
    
    public bool TransitioningVelocityY => _baseVelocityYTween != null && _baseVelocityYTween.IsValid();

    public bool TransitioningMultiplier => _multiplierTween != null && _multiplierTween.IsValid();
    
    private Tween _baseVelocityXTween;
    private bool _baseVelocityXTweenReady;
    private float _tweeningBaseVelocityXTo;
    private Tween _baseVelocityYTween;
    private bool _baseVelocityYTweenReady;
    private float _tweeningBaseVelocityYTo;
    
    private Tween _multiplierTween;
    private bool _multiplierTweenReady;
    private float _tweeningMultiplierTo;
    private float _multiplierBeforeDisable;

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
        _tweeningBaseVelocityXTo = to;
        return SmoothlySet(ref _baseVelocityXTween,
            $"{nameof(BaseVelocity)}:x", // have to hardcode "x" since "BaseVelocity:X" doesn't work
            to,
            duration);
    }

    public (Tween, PropertyTweener) SmoothlySetBaseVelocityX(float from, float to, float duration) {
        _baseVelocityXTweenReady = true;
        _tweeningBaseVelocityXTo = to;
        return SmoothlySet(ref _baseVelocityXTween,
            $"{nameof(BaseVelocity)}:x",
            from,
            to,
            duration);
    }

    public (Tween, PropertyTweener) SmoothlySetBaseVelocityY(float to, float duration) {
        _baseVelocityYTweenReady = true;
        _tweeningBaseVelocityYTo = to;
        return SmoothlySet(ref _baseVelocityYTween,
            $"{nameof(BaseVelocity)}:y",
            to,
            duration);
    }

    public (Tween, PropertyTweener) SmoothlySetBaseVelocityY(float from, float to, float duration) {
        _baseVelocityYTweenReady = true;
        _tweeningBaseVelocityYTo = to;
        return SmoothlySet(ref _baseVelocityYTween,
            $"{nameof(BaseVelocity)}:y",
            from,
            to,
            duration);
    }

    public (Tween, PropertyTweener) SmoothlySetMultiplier(float to, float duration) {
        _multiplierTweenReady = true;
        _tweeningMultiplierTo = to;
        return SmoothlySet(ref _multiplierTween, nameof(Multiplier), to, duration);
    }

    public (Tween, PropertyTweener) SmoothlySetMultiplier(float from, float to, float duration) {
        _multiplierTweenReady = true;
        _tweeningMultiplierTo = to;
        return SmoothlySet(ref _multiplierTween, nameof(Multiplier), from, to, duration);
    }

    public void SmoothlyDisable() {
        if (!Enabled) return;
        
        _multiplierBeforeDisable = Multiplier;
        (Tween t, _) = SmoothlySetMultiplier(0, SmoothlyDisableTime);
        t.Finished += () => Enabled = false;
    }

    public void SmoothlyEnable() {
        if (Enabled) return;
        
        SmoothlySetMultiplier(0, _multiplierBeforeDisable, SmoothlyEnableTime);
        Enabled = true;
    }

    public void AbortAllTransitions() {
        _baseVelocityXTween?.Kill();
        _baseVelocityXTweenReady = false;
        _baseVelocityYTween?.Kill();
        _baseVelocityYTweenReady = false;
        _multiplierTween?.Kill();
        _multiplierTweenReady = false;
    }
}