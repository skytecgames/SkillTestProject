using UnityEngine;
using System.Collections;

namespace Pathfinding.ZStar
{
    public class Graph_TilePath_ME : IGraph<Tile>
    {
        //Данные поиска пути
        private Graph_ZoneData data;

        //Карта нодов
        private Node_Tile_ME[,] nodes;

        //Коллекция нодов
        NodeCollection_Tile_ME nodeCollection;

        public Graph_TilePath_ME(Graph_ZoneData data)
        {
            this.data = data;
        }

        public void Reset(World world)
        {
            data.tilemap = new TileZoneData[world.width, world.height];

            nodes = new Node_Tile_ME[world.width, world.height];
            nodeCollection = new NodeCollection_Tile_ME(nodes);

            for (int x = 0; x < world.width; ++x) {
                for (int y = 0; y < world.height; ++y) {
                    data.tilemap[x, y] = new TileZoneData();
                    data.tilemap[x, y].tile = world[x, y];
                    nodes[x, y] = new Node_Tile_ME(world[x, y], nodes);
                }
            }
        }

        public IPathNodeCollection<Tile> GetNodes()
        {
            return nodeCollection;
        }        

        public void UpdateTile(Tile t)
        {
            //DO NOTHING
        }
    }
}