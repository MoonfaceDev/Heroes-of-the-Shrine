using UnityEngine;

public class GoblinController : CharacterBehaviour
{
    private Pathfind pathfind;
    private WalkBehaviour walkBehaviour;

    void Start()
    {
        pathfind = GetComponent<Pathfind>();
        walkBehaviour = GetComponent<WalkBehaviour>();
    }

    // Update is called once per frame
    void Update()
    {
        MovableObject angel = GameObject.Find("Angel").GetComponent<MovableObject>();
        Vector3 direction = pathfind.Direction(movableObject.position, angel.position);
        if (direction != Vector3.zero)
        {
            walkBehaviour.Walk(direction.x, direction.z);
        } else
        {
            walkBehaviour.Stop();
        }
    }
}
