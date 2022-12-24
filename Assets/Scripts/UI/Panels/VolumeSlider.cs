using UnityEngine;
using UnityEngine.UI;

public class VolumeSlider : MonoBehaviour
{
    public Slider slider;

    private void Awake()
    {
        slider.onValueChanged.AddListener(value => AudioListener.volume = value);

        EventManager.Instance.Attach(() => true, () => slider.value = AudioListener.volume);
    }
}