using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class Transition
{
    public MaskableGraphic graphic;
    public float transitionDuration;
    public Color startColor;
    public Color finalColor;
}

[RequireComponent(typeof(Image))]
public class DeathPanel : MonoBehaviour
{
    public Transition[] transitions;

    private void Start()
    {
        float startTime = Time.time;
        EventManager.Instance.Attach(() => true, () =>
        {
            foreach (Transition transition in transitions)
            {
                transition.graphic.color = Color.Lerp(transition.startColor, transition.finalColor, (Time.time - startTime) / transition.transitionDuration);
            }
        }, false);
    }
}
