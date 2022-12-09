using System;
using System.Collections.Generic;
using TypeReferences;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

[Serializable]
public class EffectDefinition
{
    public TypeReference effectType;
    public Sprite icon;
    public Color color;
}

[RequireComponent(typeof(VerticalLayoutGroup))]
public class EffectsBar : MonoBehaviour
{
    [FormerlySerializedAs("EffectBarPrefab")] public GameObject effectBarPrefab;
    public EffectDefinition[] effects;

    private VerticalLayoutGroup container;
    private Dictionary<Type, GameObject> bars;
    private GameObject player;

    public void Awake()
    {
        container = GetComponent<VerticalLayoutGroup>();
        player = GameObject.FindGameObjectWithTag("Player");
        bars = new Dictionary<Type, GameObject>();
    }

    public void Start()
    {
        foreach (var definition in effects)
        {
            var effect = player.GetComponent(definition.effectType) as BaseEffect;
            if (effect == null) continue;
            effect.OnPlay += () =>
            {
                var bar = Instantiate(effectBarPrefab, container.transform);
                bar.GetComponent<EffectBar>().effect = effect;
                bar.transform.Find("Effect Icon").GetComponent<Image>().sprite = definition.icon;
                bar.GetComponent<Scrollbar>().handleRect.GetComponent<Image>().color = definition.color;
                bars.Add(definition.effectType, bar);
            };
            effect.OnStop += () =>
            {
                Destroy(bars[definition.effectType]);
                bars.Remove(definition.effectType);
            };
        }
    }
}