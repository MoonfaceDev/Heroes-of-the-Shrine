using UnityEngine.UI;

/// <summary>
/// Class that syncs between the SFX volume and a slider
/// </summary>
public class SoundEffectsVolumeSlider : BaseComponent
{
    /// <value>
    /// Related slider
    /// </value>
    public Slider slider;

    private void Awake()
    {
        slider.onValueChanged.AddListener(value => AudioManager.Instance.SoundVolume = value);

        eventManager.Register(() => slider.value = AudioManager.Instance.SoundVolume);
    }
}