using UnityEngine.UI;

/// <summary>
/// Class that syncs between the music volume and a slider
/// </summary>
public class MusicVolumeSlider : BaseComponent
{
    /// <value>
    /// Related slider
    /// </value>
    public Slider slider;

    private void Awake()
    {
        slider.onValueChanged.AddListener(value => AudioManager.Instance.MusicVolume = value);

        Register(() => slider.value = AudioManager.Instance.MusicVolume);
    }
}