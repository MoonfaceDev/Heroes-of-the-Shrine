using System;
using System.Collections.Generic;
using TypeReferences;
using UnityEngine;
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
    public GameObject EffectBarPrefab;
    public EffectDefinition[] effects;

    private VerticalLayoutGroup container;
    private Dictionary<Type, GameObject> bars;
    private GameObject player;

    public void Awake()
    {
        container = GetComponent<VerticalLayoutGroup>();
        player = GameObject.FindGameObjectWithTag("Player");
        bars = new();
    }

    public void Start()
    {
        foreach (EffectDefinition definition in effects)
        {
            BaseEffect effect = player.GetComponent(definition.effectType) as BaseEffect;
            effect.OnPlay += () =>
            {
                GameObject bar = Instantiate(EffectBarPrefab, container.transform);
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
