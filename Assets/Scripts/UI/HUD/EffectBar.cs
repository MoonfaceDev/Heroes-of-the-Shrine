using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Scrollbar))]
public class EffectBar : MonoBehaviour
{
    public BaseEffect effect;

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