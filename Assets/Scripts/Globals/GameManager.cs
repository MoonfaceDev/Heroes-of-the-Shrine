using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
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

    public void Respawn()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        Animator playerAnimator = player.GetComponent<MovableObject>().figureObject.GetComponent<Animator>();
        playerAnimator.SetBool("respawn", true);

        EventManager.Instance.StartTimeout(() =>
        {
            fadePanel.FadeOut(fadeDuration);
            EventManager.Instance.StartTimeout(Restart, fadeDuration);
        }, fadeDelay);
    }
}
