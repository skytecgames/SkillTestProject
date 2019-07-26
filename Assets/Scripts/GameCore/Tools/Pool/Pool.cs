using UnityEngine;
using System.Collections.Generic;

public class Pool<T> where T : class
{
    public Stack<T> inactive;

    public Pool(int capasity)
    {
        inactive = new Stack<T>(capasity);
    }

    public T Spawn()
    {
        return null;
    }

    public void Despawn(T value)
    {

    }
}
