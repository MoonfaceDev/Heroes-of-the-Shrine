using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class DeathPanel : MonoBehaviour
{
    public float transitionSpeed;

    private Image image;
    private TextMeshProUGUI text;
    private bool showPanel;

    void Start()
    {
        image = GetComponent<Image>();
        text = transform.Find("Text").GetComponent<TextMeshProUGUI>();
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player)
        {
            player.GetComponent<HittableBehaviour>().OnDie += () =>
            {
                showPanel = true;
            };
        }
    }

    void Update()
    {
        if (showPanel)
        {
            image.color = Color.Lerp(image.color, Color.black, transitionSpeed * Time.deltaTime);
            text.color = Color.Lerp(text.color, Color.white, transitionSpeed * Time.deltaTime);
        }
    }
}
