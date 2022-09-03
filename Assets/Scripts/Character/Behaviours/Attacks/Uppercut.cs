public class Uppercut : NormalAttack
{
    public string altNormalAttackName;

    protected override bool CanAttack()
    {
        AttackManager attackManager = GetComponent<AttackManager>();
        return base.CanAttack() && !(attackManager.lastAttack == altNormalAttackName);
    }
}
