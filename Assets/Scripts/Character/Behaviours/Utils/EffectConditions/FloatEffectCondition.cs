public class FloatEffectCondition : IEffectCondition
{
    public float max
    {
        get { return _max; }
    }

    public float value
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

    private float _max;
    private float _value;

    public FloatEffectCondition(float max)
    {
        _max = max;
        value = 0;
    }

    public float GetProgress()
    {
        return value / max;
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
