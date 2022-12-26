using UnityEngine;

public class BossBrain : GoblinBrain
{
    private static readonly int SuperArmorHealth = Animator.StringToHash("superArmorHealth");
    private SuperArmorEffect superArmorEffect;

    public override void Awake()
    {
        base.Awake();
        superArmorEffect = GetComponent<SuperArmorEffect>();
    }

    public override void Start()
    {
        base.Start();
        if (superArmorEffect)
        {
            UpdateSuperArmorHealth();
            superArmorEffect.onHit.AddListener(UpdateSuperArmorHealth);
        }
    }

    private void UpdateSuperArmorHealth()
    {
        stateMachine.SetFloat(SuperArmorHealth, superArmorEffect.armorHealth);
    }
}