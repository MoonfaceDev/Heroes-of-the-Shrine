using UnityEngine;

public class DeathTrigger : BaseTrigger
{
    private void Awake()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player)
        {
            DieBehaviour dieBehaviour = player.GetComponent<DieBehaviour>();
            dieBehaviour.OnDie += action.Invoke;
        }
    }
}
