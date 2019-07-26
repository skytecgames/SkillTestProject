using UnityEngine;
using System.Collections.Generic;

namespace Pathfinding.ZStar
{
    public class ZoneID : MonoBehaviour
    {
        private static int lastID = 0;
        private static Stack<int> pool = new Stack<int>();

        //Генерирует новый идентификатор зоны
        public static int Gen()
        {
            //Если есть идентификатор в пуле, отдаем его
            if (pool.Count > 0) {
                return pool.Pop();
            }

            //Иначе отдаем новый идентификатор
            return ++lastID;
        }

        //Возвращает идентификатор в пул
        public static void Return(int id)
        {
            pool.Push(id);
        }
    }
}