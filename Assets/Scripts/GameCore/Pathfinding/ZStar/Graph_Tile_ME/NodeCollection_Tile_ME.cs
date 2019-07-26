using UnityEngine;
using System.Collections;

namespace Pathfinding.ZStar
{   
    public class NodeCollection_Tile_ME : IPathNodeCollection<Tile>
    {
        private Node_Tile_ME[,] nodes;

        public NodeCollection_Tile_ME(Node_Tile_ME[,] nodes)
        {
            this.nodes = nodes;
        }

        public IPathNode<Tile> GetNode(Tile data)
        {
            return nodes[data.x, data.y];
        }
    }
}