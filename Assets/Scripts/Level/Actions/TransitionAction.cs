using UnityEngine;
using UnityEngine.SceneManagement;

public class TransitionAction : BaseComponent
{
    public string sceneName;

    public void Invoke()
    {
        SceneManager.LoadScene(sceneName);
    }
}
