using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Scrollbar))]
public class SuperArmorBar : BaseComponent
{
    public SuperArmorEffect superArmorEffect;

    private Scrollbar scrollbar;

    public void Awake()
    {
        scrollbar = GetComponent<Scrollbar>();
    }

    private void Update()
    {
        if (!superArmorEffect)
        {
            Destroy(gameObject);
        }

        scrollbar.size = Mathf.Lerp(scrollbar.size, GetValue(), 3f * Time.deltaTime);
    }

    private float GetValue()
    {
        return superArmorEffect.GetProgress() != 0
            ? superArmorEffect.GetProgress()
            : (Time.time - superArmorEffect.armorCooldownStart) / superArmorEffect.armorCooldown;
    }
}