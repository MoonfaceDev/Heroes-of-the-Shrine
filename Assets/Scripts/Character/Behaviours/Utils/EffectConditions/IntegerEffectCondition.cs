public class IntegerEffectCondition : BaseEffectCondition
{
    public int max
    {
        get { return _max; }
    }

    public int value
    {
        get { return _value; }
        set
        {
            _value = value;
            if (_value >= max)
            {
                InvokeOnSet();
            }
        }
    }

    private int _max;
    private int _value;

    public IntegerEffectCondition(int max)
    {
        _max = max;
        value = 0;
    }

    public override float GetProgress()
    {
        return (float) value / max;
    }

    public override bool IsSet()
    {
        return value >= max;
    }

    public override void Set()
    {
        value = max;
    }
}
