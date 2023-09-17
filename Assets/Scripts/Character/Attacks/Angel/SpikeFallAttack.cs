using System.Collections;
using UnityEngine;

public class SpikeFallAttack : BaseAttack
{
    public float minHeight;
    public float velocity;
    public float angle;

    [SerializeInterface] [SerializeReference]
    public BaseHitDetector hitDetector;

    public ChainHitExecutor hitExecutor;

    [InjectBehaviour] private JumpBehaviour jumpBehaviour;
    private bool landed;

    protected override MotionSettings Motion => MotionSettings.WalkingDisabled;
    protected override bool IsMidair => true;

    public override bool CanPlay(Command command)
    {
        return base.CanPlay(command) && Entity.position.y > minHeight;
    }

    private void Start()
    {
        PlayEvents.onStop += FinishActive;
    }

    private void FinishActive()
    {
        hitDetector.StopDetector();
        MovableEntity.OnLand -= Land;
        MovableEntity.velocity = Vector3.zero;
    }

    protected override IEnumerator ActivePhase()
    {
        var originalDirection = MovableEntity.rotation;
        jumpBehaviour.Freeze();
        MovableEntity.velocity.x = Mathf.Cos(Mathf.Deg2Rad * angle) * originalDirection * velocity;
        MovableEntity.velocity.y = -Mathf.Sin(Mathf.Deg2Rad * angle) * velocity;
        MovableEntity.velocity.z = 0;
        MovableEntity.acceleration = Vector3.zero;

        StartHitDetector(hitDetector, hitExecutor);

        landed = false;
        MovableEntity.OnLand += Land;
        yield return new WaitUntil(() => landed);

        FinishActive();
    }

    private void Land()
    {
        landed = true;
    }
}