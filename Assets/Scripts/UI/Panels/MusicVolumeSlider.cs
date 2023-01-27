using UnityEngine.UI;

public class MusicVolumeSlider : BaseComponent
{
    public Slider slider;

    private void Awake()
    {
        slider.onValueChanged.AddListener(value => AudioManager.Instance.MusicVolume = value);

        Register(() => slider.value = AudioManager.Instance.MusicVolume);
    }
}