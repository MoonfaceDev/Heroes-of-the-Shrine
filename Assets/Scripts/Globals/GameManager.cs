using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Class including general level operations
/// </summary>
public class GameManager : BaseComponent
{
    /// <value>
    /// Name of the first scene in the level, taken from the scene file name
    /// </value>
    public string firstSceneName;
    
    /// <summary>
    /// Quits the game entirely
    /// </summary>
    public void Quit()
    {
        Application.Quit();
    }

    /// <summary>
    /// Restarts current scene
    /// </summary>
    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    /// <summary>
    /// Moves to the first scene in the level
    /// </summary>
    public void PlayAgain()
    {
        SceneManager.LoadScene(firstSceneName);
    }
}