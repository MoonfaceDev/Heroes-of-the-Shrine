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
        scrollbar.size = Mathf.Lerp(scrollbar.size, hittableBehaviour.Fraction, 3f * Time.deltaTime);
    }
}
