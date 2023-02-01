using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Bar that displays the health left in a super armor when it's active, and the "refill" progress when it's broken
/// </summary>
[RequireComponent(typeof(Scrollbar))]
public class SuperArmorBar : BaseComponent
{
    /// <value>
    /// Related <see cref="superArmor"/>
    /// </value>
    public SuperArmor superArmor;

    private Scrollbar scrollbar;

    private void Awake()
    {
        scrollbar = GetComponent<Scrollbar>();
    }

    protected override void Update()
    {
        base.Update();

        if (!superArmor)
        {
            Destroy(gameObject);
        }

        scrollbar.size = Mathf.Lerp(scrollbar.size, GetValue(), 3f * Time.deltaTime);
    }

    private float GetValue()
    {
        return superArmor.GetProgress() != 0
            ? superArmor.GetProgress()
            : (Time.time - superArmor.armorCooldownStart) / superArmor.armorCooldown;
    }
}