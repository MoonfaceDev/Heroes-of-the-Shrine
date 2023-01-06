using UnityEngine.UI;

public class SoundEffectsVolumeSlider : BaseComponent
{
    public Slider slider;

    private void Awake()
    {
        slider.onValueChanged.AddListener(value => AudioManager.Instance.soundEffectsAudioSource.volume = value);

        Register(() => slider.value = AudioManager.Instance.soundEffectsAudioSource.volume);
    }
}