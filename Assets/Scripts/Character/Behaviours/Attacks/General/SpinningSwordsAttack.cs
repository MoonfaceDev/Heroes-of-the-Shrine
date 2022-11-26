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
        EventListener disableHitDetector1 = null;
        EventListener enableHitDetector2 = null;

        generalEvents.onStartActive.AddListener(() =>
        {
            hitDetector1.StartDetector(HitCallable, AttackManager.hittableTags);
            disableHitDetector1 = EventManager.Instance.StartTimeout(hitDetector1.StopDetector, detector1FinishTime);
            enableHitDetector2 = EventManager.Instance.StartTimeout(
                () => hitDetector2.StartDetector(HitCallable, AttackManager.hittableTags),
                detector2StartTime
            );
        });

        generalEvents.onFinishActive.AddListener(() =>
        {
            hitDetector1.StopDetector();
            hitDetector2.StopDetector();
            EventManager.Instance.Detach(disableHitDetector1);
            EventManager.Instance.Detach(enableHitDetector2);
        });
    }
}