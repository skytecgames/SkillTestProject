using UnityEngine;
using System.Collections.Generic;

namespace Pathfinding.ZStar
{
    //Данные тайла, для системы поиска пути
    public class TileZoneData
    {
        public Zone zone;
        public Tile tile;

        public static readonly TileZoneData EmptyTile = new TileZoneData();
    }

    public class Graph_ZoneData
    {
        public TileZoneData[,] tilemap;
        public Dictionary<Zone, Node_Zone> zonemap;
    }
}