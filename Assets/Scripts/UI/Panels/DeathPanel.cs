using System;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class DeathPanel : BaseComponent
{
    [Serializable]
    public class Transition
    {
        public MaskableGraphic graphic;
        public float transitionDuration;
        public Color startColor;
        public Color finalColor;
    }
    
    public Transition[] transitions;

    private void Start()
    {
        var startTime = Time.time;
        Register(() =>
        {
            foreach (var transition in transitions)
            {
                transition.graphic.color = Color.Lerp(transition.startColor, transition.finalColor,
                    (Time.time - startTime) / transition.transitionDuration);
            }
        });
    }
}