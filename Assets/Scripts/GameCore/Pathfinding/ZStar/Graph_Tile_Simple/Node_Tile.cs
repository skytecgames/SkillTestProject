using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Pathfinding.ZStar
{
    public class Node_Tile : IPathNode<Tile>
    {
        private Tile tile;

        public IList<IPathNode<Tile>> edges;

        public Node_Tile(Tile t)
        {
            edges = new List<IPathNode<Tile>>();

            Reset(t);
        }

        public Tile node
        {
            get { return tile; }
        }

        public float movementCost
        {
            get { return tile.movementCost; }
        }

        public IEnumerator<IPathNode<Tile>> GetNeighbours()
        {
            return edges.GetEnumerator();
        }

        public void Reset(Tile t)
        {
            tile = t;
            edges.Clear();
        }
    }
}