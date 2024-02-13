using UnityEngine.SceneManagement;

/// <summary>
/// Loads a scene
/// </summary>
public class TransitionAction : BaseComponent
{
    /// <value>
    /// Name of the scene to load - exactly its file name
    /// </value>
    public string sceneName;

    public void Invoke()
    {
        SceneManager.LoadScene(sceneName);
    }
}