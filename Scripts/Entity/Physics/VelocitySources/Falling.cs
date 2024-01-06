using Godot;

namespace Learning.Scripts.Entity.Physics.VelocitySources;

/// <summary>
///     A source of velocity representing the falling due to gravity. A Falling's y velocity is not
///     necessarily always zero or downwards (e.g. a Jumping could set its velocity to upwards)
/// </summary>
public partial class Falling : VelocitySource
{
    /// <summary>
    ///     A signal for when this Falling goes from not falling to falling.
    /// </summary>
    [Signal]
    public delegate void StartFallingEventHandler();

    /// <summary>
    ///     A signal for when this Falling goes from falling to not falling.
    /// </summary>
    [Signal]
    public delegate void StopFallingEventHandler();

    /// <summary>
    ///     The base acceleration due to gravity.
    /// </summary>
    public readonly float Gravity =
        ProjectSettings.GetSetting("physics/2d/default_gravity").AsSingle();

    private Tween _ceilingHitStopTween;
    private Tween _decelerateToMaxVelocityTween;

    private FallingData _fallData;

    private bool _isFalling;

    private float _originalCeilingHitStopTime;

    private float _originalDecelerateVelocityDelta;

    /// <summary>
    ///     Data used by this Falling to determine acceleration, max fall velocity, etc.
    /// </summary>
    [Export]
    public FallingData FallData
    {
        get => _fallData;
        private set
        {
            _fallData = value;
            FallDataUpdated();
        }
    }

    /// <summary>
    ///     Whether or not this Falling is in a state of falling
    ///     (i.e., currently accelerating downwards).
    ///     Setting this to true (from false) causes this Falling to accelerate downwards.
    ///     Setting this to false (from true) causes the acceleration to stop and y velocity to go to 0.
    /// </summary>
    public bool IsFalling
    {
        get => _isFalling;
        set
        {
            if (IsFalling == value) return;

            _isFalling = value;
            if (IsFalling)
            {
                EmitSignal(SignalName.StartFalling);
            }
            else
            {
                BaseVelocity = new Vector2(BaseVelocity.X, 0);
                EmitSignal(SignalName.StopFalling);
            }
        }
    }

    /// <summary>
    ///     The currently-used scale applied to the base gravity.
    /// </summary>
    public float GravityScale =>
        Velocity.Y < 0 ? FallData.UpwardsGravityScale : FallData.DownwardsGravityScale;

    /// <summary>
    ///     Whether this Falling's y velocity is currently going to zero due to hitting a ceiling.
    /// </summary>
    public bool StoppingDueToCeilingHit =>
        _ceilingHitStopTween is not null && _ceilingHitStopTween.IsValid();

    /// <summary>
    ///     Whether this Falling's y velocity is currently decreasing to the max fall velocity
    ///     (from a y velocity greater than the max fall velocity).
    /// </summary>
    public bool DeceleratingToMaxFallVelocity =>
        _decelerateToMaxVelocityTween is not null && _decelerateToMaxVelocityTween.IsValid();

    public override void _Ready()
    {
        FallData.ModifierUpdated += (_, _) => FallDataUpdated();
    }

    private void FallDataUpdated()
    {
        if (IsNodeReady())
        {
            UpdateDecelerationToMaxVelocity();
            UpdateCeilingHitStop();
        }
    }

    private void UpdateDecelerationToMaxVelocity()
    {
        if (_decelerateToMaxVelocityTween != null && _decelerateToMaxVelocityTween.IsValid())
        {
            if (BaseVelocity.Y > FallData.MaxFallVelocity)
            {
                float newDuration = _originalDecelerateVelocityDelta *
                                    FallData.DecelToMaxVelocityTimePerVelocity;
                _decelerateToMaxVelocityTween.SetSpeedScale(1 / newDuration);
            }
            else
            {
                _decelerateToMaxVelocityTween.Kill();
            }
        }
    }

    private void UpdateCeilingHitStop()
    {
        if (_ceilingHitStopTween != null && _ceilingHitStopTween.IsValid())
        {
            float newDuration = _originalCeilingHitStopTime * FallData.CeilingHitStopTimeScale;
            _ceilingHitStopTween.SetSpeedScale(1 / newDuration);
        }
    }

    public override void _PhysicsProcess(double delta)
    {
        if (!Enabled) return;
        base._PhysicsProcess(delta);
        if (!IsFalling) return;

        if (BaseVelocity.Y > FallData.MaxFallVelocity)
            DecelerateToMaxFallVelocity();
        else
            AccelerateFromGravity(delta);
    }

    private void DecelerateToMaxFallVelocity()
    {
        if (_decelerateToMaxVelocityTween != null &&
            _decelerateToMaxVelocityTween.IsValid()) return;

        _originalDecelerateVelocityDelta = BaseVelocity.Y - FallData.MaxFallVelocity;
        float decelerateTime = _originalDecelerateVelocityDelta *
                               FallData.DecelToMaxVelocityTimePerVelocity;
        (_decelerateToMaxVelocityTween, PropertyTweener t)
            = SmoothlySetBaseVelocityY(FallData.MaxFallVelocity, decelerateTime);
        t.SetEase(Tween.EaseType.Out);
        t.SetTrans(Tween.TransitionType.Quad);
    }

    private void AccelerateFromGravity(double delta)
    {
        Vector2 velocity = BaseVelocity;
        velocity.Y += Gravity * GravityScale * (float)delta;
        velocity.Y = Mathf.Min(velocity.Y, FallData.MaxFallVelocity);
        BaseVelocity = velocity;
    }

    /// <summary>
    ///     Makes this Falling's y velocity go to 0 (over time),
    ///     as if it were moving upwards and then hit a ceiling.
    /// </summary>
    public void CeilingHitStop()
    {
        _originalCeilingHitStopTime = Mathf.Abs(BaseVelocity.Y) / (Gravity * GravityScale);
        (_ceilingHitStopTween, _) =
            SmoothlySetBaseVelocityY(0,
                _originalCeilingHitStopTime * FallData.CeilingHitStopTimeScale);
    }
}