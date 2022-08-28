public interface IEffectCondition
{
    public delegate void OnSet();
    public event OnSet onSet;
    float GetProgress();
    bool IsSet();
    void Set();
}
