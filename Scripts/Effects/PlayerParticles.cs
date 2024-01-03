using Godot;
using Learning.Scripts.Entity;
using Learning.Scripts.Entity.Physics;

namespace Learning.Scripts.Effects;

public partial class PlayerParticles : Node2D
{
    private bool _canMakeFloorImpact;

    private float _fallStartPositionY;
    private ImpactParticles GroundJumpParticles { get; set; }
    private ImpactParticles AirJumpParticles { get; set; }
    private ImpactParticles LeftWallJumpParticles { get; set; }
    private ImpactParticles RightWallJumpParticles { get; set; }
    private CpuParticles2D WalkingParticles { get; set; }
    private CpuParticles2D LeftWallDragParticles { get; set; }
    private CpuParticles2D RightWallDragParticles { get; set; }
    private FloorImpactParticles FloorLandingParticles { get; set; }

    public override void _Ready()
    {
        GroundJumpParticles = GetNode<ImpactParticles>(nameof(GroundJumpParticles));
        AirJumpParticles = GetNode<ImpactParticles>(nameof(AirJumpParticles));
        LeftWallJumpParticles = GetNode<ImpactParticles>(nameof(LeftWallJumpParticles));
        RightWallJumpParticles = GetNode<ImpactParticles>(nameof(RightWallJumpParticles));

        WalkingParticles = GetNode<CpuParticles2D>(nameof(WalkingParticles));
        LeftWallDragParticles = GetNode<CpuParticles2D>(nameof(LeftWallDragParticles));
        RightWallDragParticles = GetNode<CpuParticles2D>(nameof(RightWallDragParticles));

        FloorLandingParticles = GetNode<FloorImpactParticles>(nameof(FloorLandingParticles));
    }

    public void InitParticlesBehavior(Player player)
    {
        InitJumpParticleBehavior(player);
        InitWalkParticleBehavior(player);
        InitWallDragParticleBehavior(player);
        InitFloorImpactBehavior(player);
    }

    private void InitJumpParticleBehavior(Player player)
    {
        player.PlayerController.Jumping.Jumped += from =>
        {
            ImpactParticles toEmit = (from, player.PlayerController.Jumping.JumpFacing) switch
            {
                (Location.Ground, _) => GroundJumpParticles,
                (Location.Air, _) => AirJumpParticles,
                (Location.WallNonGround, > 0) => LeftWallJumpParticles,
                (Location.WallNonGround, < 0) => RightWallJumpParticles,
                _ => null
            };
            toEmit?.EmitParticles();
        };
    }

    private void InitWalkParticleBehavior(Player player)
    {
        player.PlayerController.LeftRight.GoingLeft += () => WalkCheck(player);
        player.PlayerController.LeftRight.GoingRight += () => WalkCheck(player);
        player.PlayerController.LeftRight.StopMoving += () => WalkCheck(player);
        player.BecomeOnFloor += _ => WalkCheck(player);
        player.BecomeOffFloor += _ => WalkCheck(player);
        player.BecomeOnWall += _ => WalkCheck(player);
        player.BecomeOffWall += _ => WalkCheck(player);
    }

    private void WalkCheck(Player player)
    {
        WalkingParticles.Emitting =
            player.IsOnFloor() && !player.IsOnWall() &&
            player.PlayerController.LeftRight.CurrentSpeedScale != 0;
    }

    private void InitWallDragParticleBehavior(Player player)
    {
        player.PlayerController.WallDragging.StartedDragging += () =>
        {
            CpuParticles2D toEmit = player.GetWallNormal().X switch
            {
                > 0 => LeftWallDragParticles,
                < 0 => RightWallDragParticles,
                _ => null
            };
            if (toEmit is not null) toEmit.Emitting = true;
        };
        player.PlayerController.WallDragging.StoppedDragging += () =>
        {
            LeftWallDragParticles.Emitting = false;
            RightWallDragParticles.Emitting = false;
        };
    }

    private void InitFloorImpactBehavior(Player player)
    {
        player.DirectionChangeY += (state, direction) =>
        {
            switch (direction)
            {
                case > 0:
                    RecordFallStartPosition(state);
                    break;
                case < 0:
                    _canMakeFloorImpact = false;
                    break;
            }
        };

        player.PlayerController.WallDragging.StoppedDragging +=
            () => RecordFallStartPosition(player);
        player.PlayerController.WallDragging.StartedDragging += () => _canMakeFloorImpact = false;

        player.BecomeOnFloor += state =>
        {
            if (_canMakeFloorImpact)
            {
                float fallDistance = state.Position.Y - _fallStartPositionY;
                FloorLandingParticles.FloorImpact(fallDistance);
            }
        };
    }

    private void RecordFallStartPosition(Node2D toGetPositionOf)
    {
        _fallStartPositionY = toGetPositionOf.Position.Y;
        _canMakeFloorImpact = true;
    }
}