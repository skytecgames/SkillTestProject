using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace Pathfinding.AStar
{
    public class Node_Tile_ME : IPathNode<Tile>
    {
        //Тайл, для которого создан нод
        private Tile tile;
        //TOFIX: тут нужен степок карты тайлов, а не оригинал. Иначе будут проблемы с асинхронным поиском
        //TOFIX: тут нужен интерфейс для доступа, а не сама карта тайлов, иначе ее нельзя заменить
        //       или карта нодов должна быть своей для каждого слепка
        private Node_Tile_ME[,] nodes;

        private static int[] dx = new int[] { 0, 1, 0, -1, 1, 1, -1, -1 };
        private static int[] dy = new int[] { 1, 0, -1, 0, 1, -1, -1, 1 };

        public Node_Tile_ME(Tile tile, Node_Tile_ME[,] nodes)
        {
            this.tile = tile;
            this.nodes = nodes;
        }

        //Тайл соответствующий этому ноду
        public Tile node
        {
            get { return tile; }
        }

        //Цена перемещения в тайл
        public float movementCost
        {
            get { return tile.movementCost; }
        }

        //Список нодов в которые можно попасть из текущего
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