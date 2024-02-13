using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Possible tags options
/// </summary>
public enum Tag
{
    Barrier,
    Player,
    Enemy,
    Goblin,
    Zombie,
    Boss,
    Prop,
}

/// <summary>
/// Tags property that is displayed as list in the inspector
/// </summary>
[Serializable]
public class Tags : IReadOnlyCollection<Tag>
{
    [SerializeField] private List<Tag> tags;

    public IEnumerator<Tag> GetEnumerator()
    {
        return tags.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public int Count => tags.Count;

    public bool Contains(Tag tag)
    {
        return tags.Contains(tag);
    }
}