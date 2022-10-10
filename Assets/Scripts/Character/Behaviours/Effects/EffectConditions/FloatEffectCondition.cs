using System;

public class FloatEffectCondition : BaseEffectCondition
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
                InvokeOnSet();
            }
        }
    }

    private float _max;
    private float _value;

    public FloatEffectCondition(float max)
    {
        _max = max;
        value = 0;
    }

    public override float GetProgress()
    {
        return value / max;
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
