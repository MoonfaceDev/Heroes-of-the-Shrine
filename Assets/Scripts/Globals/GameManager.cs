using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public Image fadePanel;
    public float fadeDelay;
    public float fadeDuration;

    public void FadeIn()
    {
        fadePanel.gameObject.SetActive(true);

        float startTime = Time.time;
        EventManager.Instance.Attach(() => true, () =>
        {
            fadePanel.color = Color.Lerp(Color.black, new Color(0, 0, 0, 0), (Time.time - startTime) / fadeDuration);
        }, false);

        EventManager.Instance.StartTimeout(() => fadePanel.gameObject.SetActive(false), fadeDuration);
    }

    public void FadeOut()
    {
        fadePanel.gameObject.SetActive(true);
        float startTime = Time.time;
        EventManager.Instance.Attach(() => true, () =>
        {
            fadePanel.color = Color.Lerp(new Color(0, 0, 0, 0), Color.black, (Time.time - startTime) / fadeDuration);
        }, false);
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
            FadeOut();
            EventManager.Instance.StartTimeout(Restart, fadeDuration);
        }, fadeDelay);
    }
}
