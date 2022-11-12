using UnityEngine;

public class PauseManager : MonoBehaviour
{ 
    public GameObject pauseMenu;

    public bool Paused
    {
        set
        {
            paused = value;
            Time.timeScale = paused ? 0 : 1;
            pauseMenu.SetActive(paused);
        }
        get => paused;
    }

    private bool paused;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Paused = !Paused;
        }
    }
}
