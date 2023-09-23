using UnityEngine;

public class GameMenu : MonoBehaviour
{
    public UnityEngine.UI.Button defaultSelectedButton;

    public void SetOpened(bool value)
    {
        gameObject.SetActive(value);
        PauseManager.Instance.Paused = value;
    }

    public bool Opened => gameObject.activeSelf;

    public void Open()
    {
        SetOpened(true);
        defaultSelectedButton.Select();
    }

    public void Close()
    {
        SetOpened(false);
    }
}