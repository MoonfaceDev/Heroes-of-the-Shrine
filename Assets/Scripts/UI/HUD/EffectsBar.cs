using System;
using System.Collections.Generic;
using TypeReferences;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class EffectDefinition
{
    [SerializeField, TypeOptions(SerializableOnly = true)]
    public TypeReference effectType;

    public Sprite icon;
    public Color color;
}

[RequireComponent(typeof(VerticalLayoutGroup))]
public class EffectsBar : BaseComponent
{
    public GameObject effectBarPrefab;
    public EffectDefinition[] effects;

    private VerticalLayoutGroup container;
    private Dictionary<Type, GameObject> bars;
    private GameEntity player;

    public void Awake()
    {
        container = GetComponent<VerticalLayoutGroup>();
        player = EntityManager.Instance.GetEntity(Tag.Player);
        bars = new Dictionary<Type, GameObject>();
    }

    public void Start()
    {
        foreach (var definition in effects)
        {
            var effect = player.GetComponent(definition.effectType) as IEffect;
            if (effect == null) continue;
            effect.PlayEvents.onPlay.AddListener(() =>
            {
                var bar = Instantiate(effectBarPrefab, container.transform);
                bar.GetComponent<EffectBar>().effect = effect;
                bar.transform.Find("Effect Icon").GetComponent<Image>().sprite = definition.icon;
                bar.GetComponent<Scrollbar>().handleRect.GetComponent<Image>().color = definition.color;
                bars.Add(definition.effectType, bar);
            });
            effect.PlayEvents.onStop.AddListener(() =>
            {
                Destroy(bars[definition.effectType]);
                bars.Remove(definition.effectType);
            });
        }
    }
}