using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : BaseComponent
{
    public FadePanel fadePanel;
    public float fadeDelay;
    public float fadeDuration;

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
        var player = EntityManager.Instance.GetEntity(Tag.Player);
        player.GetComponent<DieBehaviour>().Respawn();

        StartTimeout(() =>
        {
            fadePanel.FadeOut(fadeDuration);
            StartTimeout(Restart, fadeDuration);
        }, fadeDelay);
    }
}