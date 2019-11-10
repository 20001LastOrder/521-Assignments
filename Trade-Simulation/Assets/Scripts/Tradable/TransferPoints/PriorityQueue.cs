using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
public class PriorityQueue<Key, Element>
{
    private SortedDictionary<Key, Queue<Element>> _sortedDictionary;

    public PriorityQueue(){
        _sortedDictionary = new SortedDictionary<Key, Queue<Element>>();
    }

    public void Enqueue(Key key, Element element)
    {
        Queue<Element> queue;
        if(!_sortedDictionary.TryGetValue(key, out queue))
        {
            queue = new Queue<Element>();
            _sortedDictionary.Add(key, queue);
        }
        queue.Enqueue(element);
    }

    public Element Dequeue()
    {
        if (IsEmpty())
            throw new Exception("No items to Dequeue:");
        var key = _sortedDictionary.Keys.First();
        var queue = _sortedDictionary[key];
        var output = queue.Dequeue();

        if(queue.Count == 0)
        {
            _sortedDictionary.Remove(key);
        }

        return output;
    }

    public bool IsEmpty()
    {
        return _sortedDictionary.Count == 0;
    }
}
