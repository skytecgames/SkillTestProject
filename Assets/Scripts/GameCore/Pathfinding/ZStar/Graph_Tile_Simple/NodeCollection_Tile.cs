using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Pathfinding.ZStar
{
    public class NodeCollection_Tile : IPathNodeCollection<Tile>
    {
        private Node_Tile[,] tiledata;

        public NodeCollection_Tile(Node_Tile[,] data)
        {
            tiledata = data;
        }

        public IPathNode<Tile> GetNode(Tile data)
        {
            return tiledata[data.x, data.y];
        }
    }
}