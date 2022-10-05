public class GoblinController : CharacterBehaviour
{
    void Start()
    {
        GetComponent<ArcPattern>().StartPattern();
    }
}
