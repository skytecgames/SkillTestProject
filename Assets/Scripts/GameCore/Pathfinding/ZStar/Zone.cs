using UnityEngine;
using System.Collections;
using System;

namespace Pathfinding.ZStar
{
    public class Zone : IMoveCost
    {
        //Идентификатор зоны
        public int id;

        //TODO: Идентификатор зоны связности (если у двух зон один идентификатор связности, то между ними существует путь)
        public int linkId = 0;

        //условный центральны тайл зоны (нужен чтобы задавать направление поиска) (не гарантируется что этот тайл часть зоны)
        public Tile center;

        //Координаты левого нижнего угла сектора, в котором расположена зона
        public int square_x;
        public int square_y;

        //TOFIX: Тайлы с разной проходимостью можно разделять по разным зонам, это позволит задать зонам условный movementCost
        public float movementCost
        {
            get { return 1; }
        }

        public Zone()
        {
            id = ZoneID.Gen();
        }
    }
}