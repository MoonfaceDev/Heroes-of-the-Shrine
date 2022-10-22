using UnityEngine;

public class PauseListener : MonoBehaviour
{ 
    public GameObject pauseMenu;

    [HideInInspector] public bool Paused
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

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Paused = !Paused;
        }
    }
}
