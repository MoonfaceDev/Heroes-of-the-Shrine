using System;
using System.Collections.Generic;

public class PriorityQueue<T>
{

    private readonly List<Tuple<T, double>> elements = new();

    public int Count => elements.Count;

    public void Enqueue(T item, double priority)
    {
        elements.Add(Tuple.Create(item, priority));
    }

    public T Dequeue()
    {
        var bestIndex = 0;

        for (var i = 0; i < elements.Count; i++)
        {
            if (elements[i].Item2 < elements[bestIndex].Item2)
            {
                bestIndex = i;
            }
        }

        var bestItem = elements[bestIndex].Item1;
        elements.RemoveAt(bestIndex);
        return bestItem;
    }
}