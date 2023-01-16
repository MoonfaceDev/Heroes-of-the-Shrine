using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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