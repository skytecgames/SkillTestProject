using UnityEngine;
using System.Collections.Generic;

namespace Pathfinding.ZStar
{
    public class Node_Tile_ME : IPathNode<Tile>
    {
        private Tile tile;

        private Node_Tile_ME[,] nodes;

        private static int[] dx = new int[] { 0, 1, 0, -1, 1, 1, -1, -1 };
        private static int[] dy = new int[] { 1, 0, -1, 0, 1, -1, -1, 1 };

        public Node_Tile_ME(Tile tile, Node_Tile_ME[,] nodes)
        {
            this.tile = tile;
            this.nodes = nodes;
        }

        public Tile node
        {
            get { return tile; }
        }

        public float movementCost {
            get { return tile.movementCost; }
        }

        public IEnumerator<IPathNode<Tile>> GetNeighbours()
        {
            return GetNeighbours(true);
        }

        private IEnumerator<Node_Tile_ME> GetNeighbours(bool diagOK)
        {
            Node_Tile_ME cur;

            for (int i = 0; i < 4; ++i) {
                cur = nodes[tile.x + dx[i], tile.y + dy[i]];
                if (cur.movementCost < Tile.walkabilityMax) yield return cur;
            }

            if (diagOK == false) yield break;

            for (int i = 4; i < 8; ++i) {
                cur = nodes[tile.x + dx[i], tile.y + dy[i]];
                if (IsClippingCorner(cur)) continue;
                if (cur.movementCost < Tile.walkabilityMax) yield return cur;
            }
        }

        private bool IsClippingCorner(Node_Tile_ME neighbour)
        {
            //если сосед по диагонали
            if (Mathf.Abs(tile.x - neighbour.tile.x) + Mathf.Abs(tile.y - neighbour.tile.y) == 2) {
                int dX = tile.x - neighbour.tile.x;
                int dY = tile.y - neighbour.tile.y;

                if (World.current[tile.x - dX, tile.y].movementCost >= Tile.walkabilityMax) {
                    return true;
                }

                if (World.current[tile.x, tile.y - dY].movementCost >= Tile.walkabilityMax) {
                    return true;
                }
            }

            return false;
        }
    }
}