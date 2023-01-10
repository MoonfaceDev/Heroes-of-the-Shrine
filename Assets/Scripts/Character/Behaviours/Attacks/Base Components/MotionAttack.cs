public class MotionAttack : SimpleAttack
{
    public float velocity;
    public float acceleration;
    public MotionAttackFlowDefinition motionAttackFlow;

    private IAttackFlow currentAttackFlow;

    public override void Awake()
    {
        base.Awake();

        PreventWalking(true);

        var direction = 0;

        PlayEvents.onPlay.AddListener(() =>
        {
            direction = MovableObject.rotation;
            currentAttackFlow = new MotionAttackFlow(motionAttackFlow, MovableObject, direction);
        });

        attackEvents.onStartActive.AddListener(() =>
        {
            MovableObject.velocity.x = direction * velocity;
            MovableObject.velocity.z = 0;
            MovableObject.acceleration.x = -direction * acceleration;
        });

        attackEvents.onFinishActive.AddListener(() =>
        {
            MovableObject.velocity.x = 0;
            MovableObject.acceleration.x = 0;
        });
    }

    protected override IAttackFlow AttackFlow =>
        currentAttackFlow ?? new MotionAttackFlow(motionAttackFlow, MovableObject, 0);
}