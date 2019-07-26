using UnityEngine;
using System.Collections;

namespace Pathfinding
{
    //Интерфейс для объекта, который указывает соответствие между двумя типами нодов
    public interface INodeConnector<T1, T2>
    {
        //выдает нод который соответствует указанному ноду другого типа
        T2 GetNode(T1 node);
    }
}
