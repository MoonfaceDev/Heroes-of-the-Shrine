public class NormalAttack : SimpleAttack
{
    public override void Awake()
    {
        base.Awake();
        onAnticipate += () =>
        {
            WalkBehaviour walkBehaviour = GetComponent<WalkBehaviour>();
            if (walkBehaviour)
            {
                walkBehaviour.Stop(true);
            }
        };
    }
}
