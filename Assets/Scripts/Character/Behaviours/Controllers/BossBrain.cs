using UnityEngine;

public class BossBrain : GoblinBrain
{
    private static readonly int SuperArmorHealth = Animator.StringToHash("superArmorHealth");

    public override void Start()
    {
        base.Start();
        var superArmorEffect = GetComponent<SuperArmorEffect>();
        if (superArmorEffect)
        {
            superArmorEffect.onHit.AddListener(() => stateMachine.SetFloat(SuperArmorHealth, superArmorEffect.armorHealth));
        }
    }
}