using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pathfinding
{
    public interface IPathfinder<T>
    {
        //Продолжить поиск (не совсем асинхронно, просто ограниченно по времени)
        bool FindAsync();

        //Поиск пути до получения результата
        void Find();

        //Получить построенный путь
        IPath<T> GetResult();
    }
}