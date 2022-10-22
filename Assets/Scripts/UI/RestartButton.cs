using UnityEngine;
using UnityEngine.SceneManagement;

public class RestartButton : MonoBehaviour
{
    public void Click()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
