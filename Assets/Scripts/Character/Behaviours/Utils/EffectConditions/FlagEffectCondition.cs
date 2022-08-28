public class FlagEffectCondition : IEffectCondition
{
    public bool flag
    {
        get { return _flag; }
        set
        {
            _flag = value;
            if (_flag)
            {
                onSet?.Invoke();
            }
        }
    }

    public event IEffectCondition.OnSet onSet;

    private bool _flag;

    public float GetProgress()
    {
        return flag ? 1 : 0;
    }

    public bool IsSet()
    {
        return flag;
    }

    public void Set()
    {
        flag = true;
    }
}
