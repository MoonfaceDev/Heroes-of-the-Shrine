using TMPro;
using UnityEngine;

public class WaveAnnouncer : BaseComponent
{
    public TMP_Text text;
    public float duration;

    public void Activate(int waveIndex)
    {
        Destroy(gameObject, duration);
        text.text = $"Wave {waveIndex + 1} begins!";
    }
}