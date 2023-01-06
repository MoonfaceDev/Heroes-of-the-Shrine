using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Scrollbar))]
public class HealthBar : BaseComponent
{
    public HealthSystem healthSystem;
    
    private Scrollbar scrollbar;

    public void Awake()
    {
        scrollbar = GetComponent<Scrollbar>();
    }

    private void Update()
    {
        if (!healthSystem)
        {
            Destroy(gameObject);
        }
        scrollbar.size = Mathf.Lerp(scrollbar.size, healthSystem.Fraction, 3f * Time.deltaTime);
    }
}
