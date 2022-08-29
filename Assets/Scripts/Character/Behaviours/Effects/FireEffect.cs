using System.Collections;
using UnityEngine;

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
    private Coroutine damageCoroutine;
    private HittableBehaviour hittableBehaviour;

    public override void Awake()
    {
        base.Awake();
        hittableBehaviour = GetComponent<HittableBehaviour>();
    }

    public void Apply(float hitInterval, float damagePerHit)
    {
        fire = true;
        onApply?.Invoke();
        damageCoroutine = StartCoroutine(DoDamage(hitInterval, damagePerHit));
    }

    public void Cancel()
    {
        fire = false;
        onCancel?.Invoke();
        StopCoroutine(damageCoroutine);
    }

    private IEnumerator DoDamage(float hitInterval, float damagePerHit)
    {
        while (true)
        {
            yield return new WaitForSeconds(hitInterval);
            hittableBehaviour.Hit(damagePerHit);
        }
    }
}
