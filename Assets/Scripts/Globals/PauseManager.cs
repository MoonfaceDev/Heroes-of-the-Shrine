using UnityEngine;

public class PauseManager : BaseComponent
{
    public static PauseManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }

        Instance = this;
    }

    public bool Paused
    {
        set
        {
            paused = value;
            Time.timeScale = paused ? 0 : 1;
        }
        get => paused;
    }

    private bool paused;
}