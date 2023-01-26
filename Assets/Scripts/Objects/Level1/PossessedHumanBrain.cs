using UnityEngine;

public class PossessedHumanBrain : BaseComponent
{
    public Animator animator;
    private static readonly int BossPossessActive = Animator.StringToHash("BossPossessActive");

    private void Start()
    {
        var possessAttack = EntityManager.Instance.GetEntity(Tag.Boss).GetComponent<PossessAttack>();
        possessAttack.attackEvents.onStartActive += () => animator.SetBool(BossPossessActive, true);
        possessAttack.attackEvents.onFinishActive += () => animator.SetBool(BossPossessActive, false);
    }
}