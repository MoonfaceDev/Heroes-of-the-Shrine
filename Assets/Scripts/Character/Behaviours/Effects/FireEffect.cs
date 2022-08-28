public class FireEffect : CharacterBehaviour
{
    public bool fire
    {
        get => _fire;
        private set
        {
            _fire = value;
            animator.SetBool("fire", value);
        }
    }

    public delegate void OnApply();
    public delegate void OnCancel();

    public event OnApply onApply;
    public event OnCancel onCancel;

    private bool _fire;

    public void Apply()
    {
        fire = true;
        onApply?.Invoke();
    }

    public void Cancel()
    {
        fire = false;
        onCancel?.Invoke();
    }
}
