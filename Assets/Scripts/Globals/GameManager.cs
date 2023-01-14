using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : BaseComponent
{
    public FadePanel fadePanel;
    public float fadeDelay;
    public float fadeDuration;
    private static readonly int RespawnParameter = Animator.StringToHash("respawn");

    private void Start()
    {
        fadePanel.FadeIn(fadeDuration);
    }

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

    public void Respawn()
    {
        var player = GameObject.FindGameObjectWithTag("Player");
        player.GetComponent<DieBehaviour>().Respawn();

        StartTimeout(() =>
        {
            fadePanel.FadeOut(fadeDuration);
            StartTimeout(Restart, fadeDuration);
        }, fadeDelay);
    }
}