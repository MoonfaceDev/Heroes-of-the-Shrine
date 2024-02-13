using UnityEngine;

/// <summary>
/// Singleton used to pause the game
/// </summary>
public class PauseManager : BaseComponent
{
    /// <value>
    /// Singleton instance
    /// </value>
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

    /// <value>
    /// True if game is paused. Setting the property stops Unity's <see cref="Time"/>, and pauses audio.
    /// </value>
    public bool Paused
    {
        set
        {
            paused = value;
            Time.timeScale = paused ? 0 : 1;
            AudioListener.pause = paused;
        }
        get => paused;
    }

    private bool paused;
}