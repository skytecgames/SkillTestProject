using UnityEngine;
using System.Collections.Generic;

namespace Pathfinding
{
    public interface IPathNode<T> : IMoveCost
    {
        //Данные для текущего нода
        T node { get; }

        //Получить все ноды в которые можно перейти из этого нода    
        IEnumerator<IPathNode<T>> GetNeighbours();
    }
}