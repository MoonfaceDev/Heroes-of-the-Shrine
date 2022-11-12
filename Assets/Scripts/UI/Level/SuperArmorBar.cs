using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Scrollbar))]
public class SuperArmorBar : MonoBehaviour
{
    public SuperArmorHittableBehaviour hittableBehaviour;

    private Scrollbar scrollbar;

    public void Awake()
    {
        scrollbar = GetComponent<Scrollbar>();
    }

    private void Update()
    {
        scrollbar.size = Mathf.Lerp(scrollbar.size, GetValue(), 3f * Time.deltaTime);
    }

    private float GetValue()
    {
        return hittableBehaviour.Fraction != 0
            ? hittableBehaviour.Fraction
            : (Time.time - hittableBehaviour.armorCooldownStart) / hittableBehaviour.armorCooldown;
    }
}