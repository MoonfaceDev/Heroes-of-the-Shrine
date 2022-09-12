public class FlagEffectCondition : BaseEffectCondition
{
    public bool flag
    {
        get { return _flag; }
        set
        {
            _flag = value;
            if (_flag)
            {
                InvokeOnSet();
            }
        }
    }

    private bool _flag;

    public override float GetProgress()
    {
        return flag ? 1 : 0;
    }

    public override bool IsSet()
    {
        return flag;
    }

    public override void Set()
    {
        flag = true;
    }
}
