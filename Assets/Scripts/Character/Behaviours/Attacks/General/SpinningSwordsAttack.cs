using UnityEngine.Serialization;

public class SpinningSwordsAttack : NormalAttack
{
    public BaseHitDetector hitDetector1;
    public BaseHitDetector hitDetector2;

    [FormerlySerializedAs("hitbox1FinishTime")]
    public float detector1FinishTime;

    [FormerlySerializedAs("hitbox2StartTime")]
    public float detector2StartTime;

    protected override void ConfigureHitDetector()
    {
        string disableHitDetector1Timeout = null;
        string enableHitDetector2Timeout = null;

        generalEvents.onStartActive.AddListener(() =>
        {
            hitDetector1.StartDetector(HitCallable, AttackManager.hittableTags);
            disableHitDetector1Timeout = StartTimeout(hitDetector1.StopDetector, detector1FinishTime);
            enableHitDetector2Timeout = StartTimeout(
                () => hitDetector2.StartDetector(HitCallable, AttackManager.hittableTags),
                detector2StartTime
            );
        });

        generalEvents.onFinishActive.AddListener(() =>
        {
            hitDetector1.StopDetector();
            hitDetector2.StopDetector();
            Unregister(disableHitDetector1Timeout);
            Unregister(enableHitDetector2Timeout);
        });
    }
}