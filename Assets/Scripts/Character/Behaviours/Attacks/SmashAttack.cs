public class SmashAttack : NormalAttack
{
    public string spinningSwordsAttackName;

    protected override bool CanAttack()
    {
        AttackManager attackManager = GetComponent<AttackManager>();
        return base.CanAttack() && attackManager.lastAttack == spinningSwordsAttackName;
    }
}
