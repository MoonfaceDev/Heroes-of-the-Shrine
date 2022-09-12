using System;

public class NormalAttack : SimpleAttack
{
    public Hitbox hitbox;

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
        onAnticipate += () =>
        {
            WalkBehaviour walkBehaviour = GetComponent<WalkBehaviour>();
            walkBehaviour.Stop(true);
        };
        onStart += () => hitDetector.Start();
        void StopHitDetector() => hitDetector.Stop();
        onFinish += StopHitDetector;
        onStop += StopHitDetector;
    }

    protected override bool CanAttack()
    {
        RunBehaviour runBehaviour = GetComponent<RunBehaviour>();
        return base.CanAttack() && !(runBehaviour && runBehaviour.run);
    }
}
