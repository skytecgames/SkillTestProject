using UnityEngine;
using System.Collections.Generic;

namespace Pathfinding
{
    public interface IPathNodeCollection<T>
    {
        //Получить нод по данным
        IPathNode<T> GetNode(T data);
    }
}