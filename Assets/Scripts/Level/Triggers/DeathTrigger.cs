using UnityEngine;

public class DeathTrigger : BaseTrigger
{
    private void Awake()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player)
        {
            HittableBehaviour hittableBehaviour = player.GetComponent<HittableBehaviour>();
            hittableBehaviour.OnDie += action.Invoke;
        }
    }
}
