using UnityEngine;

public class ResumeButton : MonoBehaviour
{
    public PauseListener pauseListener;

    public void Click()
    {
        pauseListener.Paused = false;
    }
}
