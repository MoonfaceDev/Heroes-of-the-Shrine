using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class Transition
{
    public MaskableGraphic graphic;
    public float transitionSpeed;
    public Color finalColor;
}

[RequireComponent(typeof(Image))]
public class DeathPanel : MonoBehaviour
{
    public Transition[] transitions;

    void Update()
    {
        foreach (Transition transition in transitions)
        {
            transition.graphic.color = Color.Lerp(transition.graphic.color, transition.finalColor, transition.transitionSpeed * Time.deltaTime);
        }
    }
}
