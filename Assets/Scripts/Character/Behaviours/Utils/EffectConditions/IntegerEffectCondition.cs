public class IntegerEffectCondition : IEffectCondition
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
                onSet?.Invoke();
            }
        }
    }

    public event IEffectCondition.OnSet onSet;

    private int _max;
    private int _value;

    public IntegerEffectCondition(int max)
    {
        _max = max;
        value = 0;
    }

    public float GetProgress()
    {
        return (float) value / max;
    }

    public bool IsSet()
    {
        return value >= max;
    }

    public void Set()
    {
        value = max;
    }
}
