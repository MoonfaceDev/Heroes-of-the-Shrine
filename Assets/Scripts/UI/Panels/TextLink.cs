using UnityEngine;

public class TextLink : MonoBehaviour
{
    public string url;

    public void JumpToLink()
    {
        Application.OpenURL(url);
    }
}