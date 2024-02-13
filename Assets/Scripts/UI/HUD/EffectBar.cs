using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Single <see cref="BaseEffect{T}"/> bar
/// </summary>
[RequireComponent(typeof(Scrollbar))]
public class EffectBar : BaseComponent
{
    /// <value>
    /// Related effect instance on a character
    /// </value>
    public IEffect effect;

    private Scrollbar scrollbar;

    private void Awake()
    {
        scrollbar = GetComponent<Scrollbar>();
    }

    protected override void Update()
    {
        base.Update();
        scrollbar.size = 1 - effect.GetProgress();
    }
}