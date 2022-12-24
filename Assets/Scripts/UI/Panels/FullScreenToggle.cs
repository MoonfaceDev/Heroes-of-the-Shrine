using UnityEngine;
using UnityEngine.UI;

public class FullScreenToggle : MonoBehaviour
{
    public Toggle button;

    private void Awake()
    {
        button.onValueChanged.AddListener(value => Screen.fullScreen = value);

        EventManager.Instance.Attach(() => true, () => button.isOn = Screen.fullScreen);
    }
}