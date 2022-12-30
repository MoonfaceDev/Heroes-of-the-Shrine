using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
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
        var playerAnimator = player.GetComponent<MovableObject>().figureObject.GetComponent<Animator>();
        playerAnimator.SetBool(RespawnParameter, true);

        EventManager.Instance.StartTimeout(() =>
        {
            fadePanel.FadeOut(fadeDuration);
            EventManager.Instance.StartTimeout(Restart, fadeDuration);
        }, fadeDelay);
    }
}
