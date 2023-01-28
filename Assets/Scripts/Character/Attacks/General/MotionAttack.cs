using System;
using System.Collections;
using UnityEngine;

/// <summary>
/// An attack that has a single hit detector, hit executor, and has an anticipation and recovery phases with fixed
/// duration. During the active phase, the character moves in the direction it looked at with a reducing speed, until it
/// reaches zero.
/// </summary>
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

    [SerializeInterface] [SerializeReference]
    public BaseHitDetector hitDetector;

    public ChainHitExecutor hitExecutor;

    private int originalDirection;

    protected override MotionSettings Motion => MotionSettings.WalkingDisabled;

    private void Start()
    {
        PlayEvents.onStop += FinishActive;
    }

    private void FinishActive()
    {
        hitDetector.StopDetector();
        MovableEntity.velocity.x = 0;
        MovableEntity.acceleration.x = 0;
    }

    protected override IEnumerator AnticipationPhase()
    {
        originalDirection = MovableEntity.rotation;
        yield return new WaitForSeconds(attackFlow.anticipationDuration);
    }

    protected override IEnumerator ActivePhase()
    {
        MovableEntity.velocity.x = originalDirection * attackFlow.velocity;
        MovableEntity.velocity.z = 0;
        MovableEntity.acceleration.x = -originalDirection * attackFlow.acceleration;
        hitDetector.StartDetector(hittable => hitExecutor.Execute(this, hittable), AttackManager.hittableTags);
        yield return new WaitUntil(() =>
            Mathf.Approximately(MovableEntity.velocity.x, 0) ||
            !Mathf.Approximately(Mathf.Sign(MovableEntity.velocity.x), originalDirection)
        );
        FinishActive();
    }

    protected override IEnumerator RecoveryPhase()
    {
        yield return new WaitForSeconds(attackFlow.recoveryDuration);
    }
}