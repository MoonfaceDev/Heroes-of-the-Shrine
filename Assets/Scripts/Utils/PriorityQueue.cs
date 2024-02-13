using System;
using System.Collections.Generic;

/// <summary>
/// Generic priority queue
/// </summary>
/// <typeparam name="T">Element type</typeparam>
public class PriorityQueue<T>
{
    private readonly List<Tuple<T, double>> elements = new();

    /// <summary>
    /// Count of elements in the queue
    /// </summary>
    public int Count => elements.Count;

    /// <summary>
    /// Adds an item to the queue
    /// </summary>
    public void Enqueue(T item, double priority)
    {
        elements.Add(Tuple.Create(item, priority));
    }

    /// <summary>
    /// Removes the item first item from the queue, and returns it 
    /// </summary>
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