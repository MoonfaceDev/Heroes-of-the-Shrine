using UnityEngine;
using UnityEngine.UI;

public class MusicVolumeSlider : MonoBehaviour
{
    public Slider slider;

    private void Awake()
    {
        slider.onValueChanged.AddListener(value => AudioManager.Instance.musicAudioSource.volume = value);

        EventManager.Instance.Attach(() => true, () => slider.value = AudioManager.Instance.musicAudioSource.volume);
    }
}