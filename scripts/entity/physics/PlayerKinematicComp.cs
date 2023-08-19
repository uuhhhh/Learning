namespace Learning.scripts.entity.physics;

public partial class PlayerKinematicComp : KinematicComp2 {
    public Falling Falling { get; private set; }
    public LeftRight LeftRight { get; private set; }
    public Jumping Jumping { get; private set; }

    public override void _Ready() {
        Falling = GetNode<Falling>(nameof(Falling));
        LeftRight = GetNode<LeftRight>(nameof(LeftRight));
        Jumping = GetNode<Jumping>(nameof(Jumping));
        base._Ready();

        BecomeOnFloor += _ => { Jumping.ResetNumJumps(); };
        BecomeOnWall += _ => { Jumping.ResetNumJumps(); };
    }
}