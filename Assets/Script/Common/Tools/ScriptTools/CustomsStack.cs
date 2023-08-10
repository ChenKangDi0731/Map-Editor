using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 自作スタック
/// </summary>
/// <typeparam name="T"></typeparam>
public class CustomsStack<T>
{
    public int Capacity { get; set; }
    List<T> elementList;

    public int Count { get { if (elementList == null) elementList = new List<T>();return elementList.Count; } }

    public CustomsStack() { }

    public CustomsStack(int capacity)
    {
        Capacity = capacity;
    }

    public CustomsStack(CustomsStack<T> stack)
    {
        if (stack == null) return;
        Capacity = stack.Capacity;
        elementList = new List<T>(stack.ToArray());
    }

    public void SetCapacity(int capacity)
    {
        Capacity = capacity;
    }

    public void Push(T element)
    {
        if (elementList == null) elementList = new List<T>();

        if (Count >= Capacity && Count != 0)
        {
            elementList.RemoveAt(0);
        }
        elementList.Add(element);
    }

    public T Pop()
    {
        if (elementList == null || Count == 0)
        {
            Debug.LogError("Stack is empty");
            return default(T);
        }
        T element = elementList[Count - 1];
        elementList.RemoveAt(Count - 1);

        return element;
    }

    public T Peek()
    {
        if (elementList == null) return default(T);
        if (elementList==null || Count == 0)
        {
            Debug.LogError("Stack is empty");
            return default(T);
        }
        T element = elementList[Count - 1];

        return element;
    }

    public void Clear()
    {
        if (elementList != null)
        {
            elementList.Clear();
        }
    }

    public T[] ToArray()
    {
        if (elementList == null) elementList = new List<T>();
        return elementList.ToArray();
    }

}
