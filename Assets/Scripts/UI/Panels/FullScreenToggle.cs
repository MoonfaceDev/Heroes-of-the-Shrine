using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Toggles full screen for game window. This class syncs between the <see cref="Toggle"/> state and the "full screen state".
/// </summary>
public class FullScreenToggle : BaseComponent
{
    /// <value>
    /// Related checkbox
    /// </value>
    public Toggle button;

    private void Awake()
    {
        button.onValueChanged.AddListener(value => Screen.fullScreen = value);

        Register(() => button.isOn = Screen.fullScreen);
    }
}