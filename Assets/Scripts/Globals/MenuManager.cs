using UnityEngine;

public class MenuManager : BaseComponent
{
    public GameObject pausePanel;
    public GameObject optionsPanel;

    protected override void Update()
    {
        base.Update();
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            HandleEscape();
        }
    }

    private void HandleEscape()
    {
        if (optionsPanel.activeSelf)
        {
            ToggleOptionsPanel(false);
            return;
        }

        if (pausePanel.activeSelf)
        {
            TogglePausePanel(false);
            return;
        }

        TogglePausePanel(true);
    }

    public void TogglePausePanel(bool active)
    {
        pausePanel.SetActive(active);
        PauseManager.Instance.Paused = active;
    }

    public void ToggleOptionsPanel(bool active)
    {
        optionsPanel.SetActive(active);
        PauseManager.Instance.Paused = active;
    }
}