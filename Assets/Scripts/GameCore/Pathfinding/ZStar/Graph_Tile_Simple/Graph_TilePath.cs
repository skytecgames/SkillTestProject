using UnityEngine;

namespace Pathfinding.ZStar
{
    public class Graph_TilePath : IGraph<Tile>
    {
        private Graph_ZoneData data;

        private Node_Tile[,] nodemap;
        private Pool_NodeTile pool = new Pool_NodeTile();

        public Graph_TilePath(Graph_ZoneData data)
        {
            this.data = data;
        }

        public void Reset(World world)
        {
            data.tilemap = new TileZoneData[world.width, world.height];
            nodemap = new Node_Tile[world.width, world.height];

            for (int x = 0; x < world.width; ++x) {
                for (int y = 0; y < world.height; ++y) {
                    data.tilemap[x, y] = new TileZoneData();
                    Tile t = world[x, y];

                    data.tilemap[x, y].tile = t;

                    //Создаем ноды только для проходимых тайлов
                    if (t.movementCost < Tile.walkabilityMax) {
                        Path_Node<Tile> n = new Path_Node<Tile>();
                        n.data = t;

                        nodemap[x, y] = pool.Get(t);
                    }
                }
            }

            for (int x = 0; x < world.width; ++x) {
                for (int y = 0; y < world.height; ++y) {
                    //ищем соседние ноды
                    if (nodemap[x, y] != null) {
                        CalculateEdgesForTileNode(nodemap[x, y]);
                    }
                }
            }
        }

        public IPathNodeCollection<Tile> GetNodes()
        {
            return new NodeCollection_Tile(nodemap);
        }

        public void UpdateTile(Tile node)
        {
            int x = node.x;
            int y = node.y;

            //TOFIX: пул для нодов

            //Если тайл проходимый, но его нет на карте - создаем его
            if (node.movementCost < Tile.walkabilityMax && nodemap[x, y] == null) {
                //nodemap[x, y] = new Node_Tile(node);
                nodemap[x, y] = pool.Get(node);
                CalculateEdgesForTileNode(nodemap[x, y]);
            }

            //Если тайл непроходимый, но есть на карте, то удаляем его
            if (node.movementCost >= Tile.walkabilityMax && nodemap[x, y] != null) {
                ClearEdgesForTileNode(nodemap[x, y]);
                pool.Put(nodemap[x, y]);
                nodemap[x, y] = null;
            }
        }

        //Расчет связей для нода
        private void CalculateEdgesForTileNode(Node_Tile n)
        {
            Tile[] neighbours = n.node.GetNeighbours(true);

            for (int i = 0; i < neighbours.Length; ++i) {
                if (neighbours[i] != null && neighbours[i].movementCost < Tile.walkabilityMax) {

                    if (IsClippingCorner(n.node, neighbours[i])) {
                        continue;
                    }

                    Node_Tile edge = nodemap[neighbours[i].x, neighbours[i].y];
                    n.edges.Add(edge);

                    //Также соседу прописываем связь с текущим нодом
                    edge.edges.Add(n);
                }
            }
        }

        private void ClearEdgesForTileNode(Node_Tile n)
        {
            //Если нод пустой или без грайней, то ничего не делаем        
            if (n == null) return;
            if (n.edges == null) return;

            //Перебираем грани и удаляем
            for (int i = 0; i < n.edges.Count; ++i) {

                //получаем кординаты соседа
                int x = n.edges[i].node.x;
                int y = n.edges[i].node.y;

                //удаляемся из его граней
                nodemap[x, y].edges.Remove(n);
            }

            //очищаем свои грани
            n.edges.Clear();
        }

        private bool IsClippingCorner(Tile curr, Tile neighbour)
        {
            //если сосед по диагонали
            if (Mathf.Abs(curr.x - neighbour.x) + Mathf.Abs(curr.y - neighbour.y) == 2) {
                int dX = curr.x - neighbour.x;
                int dY = curr.y - neighbour.y;

                if (World.current[curr.x - dX, curr.y].movementCost >= Tile.walkabilityMax) {
                    return true;
                }

                if (World.current[curr.x, curr.y - dY].movementCost >= Tile.walkabilityMax) {
                    return true;
                }
            }

            return false;
        }
    }
}