using UnityEngine;
using System.Collections;

namespace Pathfinding
{
    public interface IPath<T>
    {
        //Длинна оставшегося пути
        int Length();

        //Получить следующий тайл пути
        T GetNext();

        //Очистка пути
        void Clear();
    }
}