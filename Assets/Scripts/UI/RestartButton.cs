using UnityEngine;
using UnityEngine.SceneManagement;

public class RestartButton : MonoBehaviour
{
    public PauseListener pauseListener;

    public void Click()
    {
        pauseListener.Paused = false;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
