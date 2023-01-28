using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Scrollbar))]
public class SuperArmorBar : BaseComponent
{
    public SuperArmorEffect superArmorEffect;

    private Scrollbar scrollbar;

    private void Awake()
    {
        scrollbar = GetComponent<Scrollbar>();
    }

    protected override void Update()
    {
        base.Update();
        
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