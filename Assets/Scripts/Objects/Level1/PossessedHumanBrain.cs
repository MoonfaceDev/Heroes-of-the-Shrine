using UnityEngine;

public class PossessedHumanBrain : BaseComponent
{
    public Animator animator;
    public string bossTag;
    private static readonly int BossPossessActive = Animator.StringToHash("BossPossessActive");

    private void Start()
    {
        var possessAttack = GameObject.FindWithTag(bossTag).GetComponent<PossessAttack>();
        possessAttack.attackEvents.onStartActive.AddListener(() => animator.SetBool(BossPossessActive, true));
        possessAttack.attackEvents.onFinishActive.AddListener(() => animator.SetBool(BossPossessActive, false));
    }
}