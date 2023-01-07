public class SpinningSwordsAttack : NormalAttack
{
    public BaseHitDetector hitDetector1;
    public BaseHitDetector hitDetector2;
    public float detector1FinishTime;
    public float detector2StartTime;

    protected override void ConfigureHitDetector()
    {
        string disableHitDetector1Timeout = null;
        string enableHitDetector2Timeout = null;

        attackEvents.onStartActive.AddListener(() =>
        {
            hitDetector1.StartDetector(HitCallable, AttackManager.hittableTags);
            disableHitDetector1Timeout = StartTimeout(hitDetector1.StopDetector, detector1FinishTime);
            enableHitDetector2Timeout = StartTimeout(
                () => hitDetector2.StartDetector(HitCallable, AttackManager.hittableTags),
                detector2StartTime
            );
        });

        attackEvents.onFinishActive.AddListener(() =>
        {
            hitDetector1.StopDetector();
            hitDetector2.StopDetector();
            Unregister(disableHitDetector1Timeout);
            Unregister(enableHitDetector2Timeout);
        });
    }
}