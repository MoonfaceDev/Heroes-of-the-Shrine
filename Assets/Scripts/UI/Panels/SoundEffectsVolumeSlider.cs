using UnityEngine;
using UnityEngine.UI;

public class SoundEffectsVolumeSlider : MonoBehaviour
{
    public Slider slider;

    private void Awake()
    {
        slider.onValueChanged.AddListener(value => AudioManager.Instance.soundEffectsAudioSource.volume = value);

        EventManager.Instance.Attach(() => true, () => slider.value = AudioManager.Instance.soundEffectsAudioSource.volume);
    }
}