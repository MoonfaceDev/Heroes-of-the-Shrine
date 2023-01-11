using System;
using System.Collections;
using UnityEngine;

public class MotionAttack : BaseAttack
{
    [Serializable]
    public class AttackFlow
    {
        public float anticipationDuration;
        public float velocity;
        public float acceleration;
        public float recoveryDuration;
    }
    
    public AttackFlow attackFlow;

    public BaseHitDetector hitDetector;

    public SimpleHitExecutor hitExecutor;

    private int originalDirection;

    protected override MotionSettings Motion => MotionSettings.WalkingDisabled;

    public void Start()
    {
        PlayEvents.onStop.AddListener(FinishActive);
    }

    private void FinishActive()
    {
        hitDetector.StopDetector();
        MovableObject.velocity.x = 0;
        MovableObject.acceleration.x = 0;
    }

    protected override IEnumerator AnticipationPhase()
    {
        originalDirection = MovableObject.rotation;
        yield return new WaitForSeconds(attackFlow.anticipationDuration);
    }

    protected override IEnumerator ActivePhase()
    {
        MovableObject.velocity.x = originalDirection * attackFlow.velocity;
        MovableObject.velocity.z = 0;
        MovableObject.acceleration.x = -originalDirection * attackFlow.acceleration;
        hitDetector.StartDetector(hittable => hitExecutor.Execute(this, hittable), AttackManager.hittableTags);
        yield return new WaitUntil(() =>
            Mathf.Approximately(MovableObject.velocity.x, 0) ||
            !Mathf.Approximately(Mathf.Sign(MovableObject.velocity.x), originalDirection)
        );
        FinishActive();
    }

    protected override IEnumerator RecoveryPhase()
    {
        yield return new WaitForSeconds(attackFlow.recoveryDuration);
    }
}