using UnityEngine;
using System.Collections;

namespace Pathfinding.AStar
{
    public class Graph_Tile_ME : IGraph<Tile>
    {
        private Node_Tile_ME[,] nodes;
        private NodeCollection_ME collection;

        public IPathNodeCollection<Tile> GetNodes()
        {
            return collection;
        }

        public void Reset(World world)
        {
            nodes = new Node_Tile_ME[world.width, world.height];
            collection = new NodeCollection_ME(nodes);

            for (int x = 0; x < world.width; ++x) {
                for (int y = 0; y < world.height; ++y) {
                    nodes[x, y] = new Node_Tile_ME(world[x, y], nodes);
                }
            }
        }

        public void UpdateTile(Tile t)
        {
            //DO NOTHING
        }
    }
}