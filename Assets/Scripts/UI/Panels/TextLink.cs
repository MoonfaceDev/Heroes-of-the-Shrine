using UnityEngine;

public class TextLink : BaseComponent
{
    public string url;

    public void JumpToLink()
    {
        Application.OpenURL(url);
    }
}