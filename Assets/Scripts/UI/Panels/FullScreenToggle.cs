using UnityEngine;
using UnityEngine.UI;

public class FullScreenToggle : BaseComponent
{
    public Toggle button;

    private void Awake()
    {
        button.onValueChanged.AddListener(value => Screen.fullScreen = value);

        Register(() => button.isOn = Screen.fullScreen);
    }
}