using UnityEngine;
using System.Collections;

namespace Pathfinding.ZStar
{
    //Класс для работы с квадратной областью с тайлами (не обязательно связанными)
    public class SquareZone
    {
        public const int size = 8;

        public int x_min;
        public int y_min;

        public int x_max { get { return x_min + size; } }
        public int y_max { get { return y_min + size; } }

        private int[,] map;

        //Создает чистую зону в которую входит тайл с указанными координатами
        public void Reset(int x, int y)
        {
            //определяем границы зоны
            x_min = (int)(x / size) * size;
            y_min = (int)(y / size) * size;

            //пересоздаем карту помеченных элементов
            if (map == null || map.GetLength(0) != size || map.GetLength(1) != size) {
                map = new int[size, size];
            }

            //сбрасываем значения в карте помеченных элементов
            for (int i = 0; i < size; ++i) {
                for (int j = 0; j < size; ++j) {
                    map[i, j] = 0;
                }
            }
        }

        public void Mark(int x, int y)
        {
            if (isInside(x, y) == false) {
                Debug.LogErrorFormat("tile [{0},{1}] outsize of square zone [{2},{3},{4},{5}",
                    x, y, x_min, y_min, x_min + size, y_min + size);
                return;
            }

            map[x - x_min, y - y_min] = 1;
        }

        //Возвращает true если элемент вне зоны или отмечен
        public bool isMarkOrOutside(int x, int y)
        {
            //Если вне зоны, то возвращаем 
            if (isInside(x, y) == false) return true;

            return map[x - x_min, y - y_min] == 1;
        }

        public bool isInside(int x, int y)
        {
            return (x >= x_min) && (x < x_min + size) && (y >= y_min) && (y < y_min + size);
        }
    }
}