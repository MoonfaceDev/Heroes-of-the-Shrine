public class AltCounterAttack : NormalAttack
{
    protected override void DoPlay(Command command)
    {
        Entity.rotation = -Entity.rotation;
        base.DoPlay(command);
    }
}