using UnityEngine;
using UnityEngine.UI;

public class FadePanel : MonoBehaviour
{
    public void FadeIn(float fadeDuration)
    {
        var panel = GetComponent<Image>();
        gameObject.SetActive(true);

        var startTime = Time.time;
        EventManager.Instance.Attach(() => true, () =>
        {
            panel.color = Color.Lerp(Color.black, new Color(0, 0, 0, 0), (Time.time - startTime) / fadeDuration);
        }, false);

        EventManager.Instance.StartTimeout(() => panel.gameObject.SetActive(false), fadeDuration);
    }

    public void FadeOut(float fadeDuration)
    {
        var panel = GetComponent<Image>();
        gameObject.SetActive(true);

        var startTime = Time.time;
        EventManager.Instance.Attach(() => true, () =>
        {
            panel.color = Color.Lerp(new Color(0, 0, 0, 0), Color.black, (Time.time - startTime) / fadeDuration);
        }, false);
    }
}
