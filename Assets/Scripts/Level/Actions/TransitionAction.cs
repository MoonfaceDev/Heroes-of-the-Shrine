using UnityEngine;
using UnityEngine.SceneManagement;

public class TransitionAction : MonoBehaviour
{
    public string sceneName;

    public void Invoke()
    {
        SceneManager.LoadScene(sceneName);
    }
}
