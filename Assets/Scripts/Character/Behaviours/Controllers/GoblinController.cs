public class GoblinController : CharacterBehaviour
{
    void Start()
    {
        GetComponent<AttackPattern>().StartPattern();
    }
}
