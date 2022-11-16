public class NormalAttack : SimpleAttack
{
    public override void Awake()
    {
        base.Awake();
        PreventWalking(true);
    }
}
