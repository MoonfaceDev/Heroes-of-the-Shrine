using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Scrollbar))]
public class EffectBar : BaseComponent
{
    public IEffect effect;

    private Scrollbar scrollbar;

    public void Awake()
    {
        scrollbar = GetComponent<Scrollbar>();
    }

    private void Update()
    {
        scrollbar.size = 1 - effect.GetProgress();
    }
}