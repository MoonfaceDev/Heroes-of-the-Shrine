public class AltNormalAttack : NormalAttack
{
    public string normalAttackName;

    protected override bool CanAttack()
    {
        AttackManager attackManager = GetComponent<AttackManager>();
        return base.CanAttack() && !(attackManager.lastAttack == normalAttackName);
    }
}
