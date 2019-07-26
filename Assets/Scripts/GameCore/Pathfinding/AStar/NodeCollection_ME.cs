using UnityEngine;
using UnityEditor;

namespace Pathfinding.AStar
{
    public class NodeCollection_ME : IPathNodeCollection<Tile>
    {
        Node_Tile_ME[,] nodes;

        public NodeCollection_ME(Node_Tile_ME[,] nodes)
        {
            this.nodes = nodes;
        }

        public IPathNode<Tile> GetNode(Tile data)
        {
            return nodes[data.x, data.y];
        }
    }
}