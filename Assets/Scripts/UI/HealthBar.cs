using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Scrollbar))]
public class HealthBar : MonoBehaviour
{
    private Scrollbar scrollbar;
    private HealthSystem healthSystem;

    public void Awake()
    {
        scrollbar = GetComponent<Scrollbar>();
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player)
        {
            healthSystem = player.GetComponent<HealthSystem>();
        }
    }

    void Update()
    {
        scrollbar.size = Mathf.Lerp(scrollbar.size, healthSystem.Fraction, 3f * Time.deltaTime);
    }
}
