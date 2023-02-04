using System;
using System.Collections.Generic;
using TypeReferences;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Spawns <see cref="EffectBar"/> objects when player receives effects
/// </summary>
[RequireComponent(typeof(VerticalLayoutGroup))]
public class EffectsBar : BaseComponent
{
    /// <summary>
    /// Definition for effect and its display properties
    /// </summary>
    [Serializable]
    public class EffectDefinition
    {
        /// <value>
        /// Type of the effect, inheriting from <see cref="BaseEffect{T}"/>
        /// </value>
        [SerializeField, TypeOptions(SerializableOnly = true)]
        public TypeReference effectType;

        /// <value>
        /// Icon displayed next to the bar
        /// </value>
        public Sprite icon;
        
        /// <value>
        /// Bar filling color
        /// </value>
        public Color color;
    }
    
    /// <value>
    /// Prefab of a single effect bar
    /// </value>
    public GameObject effectBarPrefab;
    
    /// <value>
    /// Effects that are displayed if applied to the character
    /// </value>
    public EffectDefinition[] effects;

    private VerticalLayoutGroup container;
    private Dictionary<Type, GameObject> bars;
    private GameEntity player;

    private void Awake()
    {
        container = GetComponent<VerticalLayoutGroup>();
        player = EntityManager.Instance.GetEntity(Tag.Player);
        bars = new Dictionary<Type, GameObject>();
    }

    private void Start()
    {
        foreach (var definition in effects)
        {
            var effect = player.GetBehaviour(definition.effectType) as IEffect;
            if (effect == null) continue;
            effect.PlayEvents.onPlay += () =>
            {
                var bar = Instantiate(effectBarPrefab, container.transform);
                bar.GetComponent<EffectBar>().effect = effect;
                bar.transform.Find("Effect Icon").GetComponent<Image>().sprite = definition.icon;
                bar.GetComponent<Scrollbar>().handleRect.GetComponent<Image>().color = definition.color;
                bars.Add(definition.effectType, bar);
            };
            effect.PlayEvents.onStop += () =>
            {
                Destroy(bars[definition.effectType]);
                bars.Remove(definition.effectType);
            };
        }
    }
}