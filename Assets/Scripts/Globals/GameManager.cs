using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Class including general level operations
/// </summary>
public class GameManager : BaseComponent
{
    public void Quit()
    {
        Application.Quit();
    }

    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void PlayAgain(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}