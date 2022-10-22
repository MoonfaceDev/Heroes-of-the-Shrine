using UnityEngine.SceneManagement;

public class TransitionAction : BaseAction
{
    public string sceneName;

    public override void Invoke()
    {
        SceneManager.LoadScene(sceneName);
    }
}
