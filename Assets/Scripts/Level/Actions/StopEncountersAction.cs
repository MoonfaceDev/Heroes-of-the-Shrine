public class StopEncountersAction : BaseComponent
{
    public void Invoke()
    {
        foreach (var encounter in FindObjectsOfType<EncounterAction>())
        {
            encounter.Stop();
        }
    }
}