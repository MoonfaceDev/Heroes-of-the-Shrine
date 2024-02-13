using UnityEngine;

/// <summary>
/// Opens a give URL on a web browser
/// </summary>
public class TextLink : BaseComponent
{
    /// <value>
    /// URL to open
    /// </value>
    public string url;

    /// <summary>
    /// Opens the URL, call it on button click
    /// </summary>
    public void JumpToLink()
    {
        Application.OpenURL(url);
    }
}