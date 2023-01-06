using UnityEngine;

public class EnemyDestroyAction : BaseComponent
{
    public string[] tags;

    public void Invoke()
    {
        var characters = CachedObjectsManager.Instance.GetObjects<Character>("Enemy");
        foreach (var character in characters)
        {
            DestroyIfIncluded(character);
        }
    }

    private void DestroyIfIncluded(Character character)
    {
        foreach (var group in tags)
        {
            var testedTag = character.tag;
            if (!(testedTag == group || testedTag.StartsWith(group + "."))) continue;
            var dieBehaviour = character.GetComponent<DieBehaviour>();
            if (!dieBehaviour)
            {
                Debug.LogWarning("No death behaviour!");
            }

            dieBehaviour.Kill();
            return;
        }
    }
}