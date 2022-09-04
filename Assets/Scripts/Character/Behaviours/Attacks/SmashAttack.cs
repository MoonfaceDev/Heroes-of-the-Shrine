public class SmashAttack : SimpleAttack
{
    public Hitbox hitbox;
    public string spinningSwordsAttackName;

    private SingleHitDetector hitDetector;

    public override void Awake()
    {
        base.Awake();
        hitDetector = new(eventManager, hitbox, (hit) =>
        {
            HittableBehaviour hittableBehaviour = hit.GetComponent<HittableBehaviour>();
            if (hittableBehaviour)
            {
                HitCallable(hittableBehaviour);
            }
        });
        onStart += () =>
        {
            hitDetector.Start();
        };
        onFinish += () =>
        {
            hitDetector.Stop();
        };
    }

    protected override bool CanAttack()
    {
        AttackManager attackManager = GetComponent<AttackManager>();
        return base.CanAttack() && attackManager.lastAttack == spinningSwordsAttackName;
    }
}
