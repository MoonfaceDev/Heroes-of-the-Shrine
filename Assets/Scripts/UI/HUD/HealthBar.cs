using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Bar that displays the part of health left in <see cref="healthSystem"/>
/// </summary>
[RequireComponent(typeof(Scrollbar))]
public class HealthBar : BaseComponent
{
    /// <value>
    /// Related <see cref="healthSystem"/>
    /// </value>
    public HealthSystem healthSystem;

    private Scrollbar scrollbar;

    private void Awake()
    {
        scrollbar = GetComponent<Scrollbar>();
    }

    protected override void Update()
    {
        base.Update();
        if (!healthSystem)
        {
            Destroy(gameObject);
        }

        scrollbar.size = Mathf.Lerp(scrollbar.size, healthSystem.Fraction, 3f * Time.deltaTime);
    }
}