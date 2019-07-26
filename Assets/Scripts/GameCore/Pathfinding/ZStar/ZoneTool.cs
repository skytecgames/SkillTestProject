using Priority_Queue;
using System.Collections.Generic;

namespace Pathfinding.ZStar
{
    public class ZoneTool
    {
        private SquareZone closed = new SquareZone();        

        //Данные графа
        private TileZoneData[,] tiledata;
        private Dictionary<Zone, Node_Zone> zonemap;

        //Пул нодов
        private Pool_NodeZone pool_zone;

        //Крайние тайлы зоны
        private Tile tile_w;
        private Tile tile_e;
        private Tile tile_n;
        private Tile tile_s;

        //SubTools
        private ZoneTool_Link tool_link;

        public ZoneTool(TileZoneData[,] tiledata, Dictionary<Zone, Node_Zone> zonemap)
        {
            //this.graph = graph;
            this.tiledata = tiledata;
            this.zonemap = zonemap;

            pool_zone = new Pool_NodeZone();

            tool_link = new ZoneTool_Link(zonemap);
        }

        //Расчитывает зону в указаной точке (не проверяет необходимость этого действия)
        public Node_Zone CreateZone(int x, int y)
        {
            //генерация нового нода зоны
            Node_Zone result = pool_zone.Get();

            //Инициализируем крайние тайлы
            tile_w = tile_e = tile_n = tile_s = tiledata[x, y].tile;

            //инициализируем закрытый список
            closed.Reset(x, y);

            //устанавливаем координаты угла сектора в котором расположена зона
            result.data.square_x = closed.x_min;
            result.data.square_y = closed.y_min;

            //поиск по тайлам зоны
            //TOTHINK: можно заменить priorityQueue на Queue(size*size)
            SimplePriorityQueue<TileZoneData> openlist = new SimplePriorityQueue<TileZoneData>();

            //Добавляем стартовый тайл в открытий и закрытый списки
            openlist.Enqueue(tiledata[x, y], 0);
            closed.Mark(x, y);

            //Добавляем стартовый тайл в создаваемую зону связности
            tiledata[x, y].zone = result.data;

            int f = 0;

            while (openlist.Count > 0) {
                //Берем нод из списка открытых
                TileZoneData current = openlist.Dequeue();

                //берем соседние ноды от текущего
                TileZoneData[] ns = GetNeighbours(current, false);

                for (int i = 0; i < ns.Length; ++i) {
                    //Если тайла нет
                    if (ns[i].tile == null) continue;

                    //Если тайл вне границ поиска и он принадлежит другой зоне, то нужно создать связь
                    if (closed.isInside(ns[i].tile.x, ns[i].tile.y) == false && ns[i].zone != null) {
                        //result.Link(graph.GetNode(ns[i].zone));
                        result.Link(zonemap[ns[i].zone]);
                        continue;
                    }

                    //Если тайл уже в закрытом списке
                    if (closed.isMarkOrOutside(ns[i].tile.x, ns[i].tile.y)) continue;

                    //Добавляем в закрытый список
                    closed.Mark(ns[i].tile.x, ns[i].tile.y);

                    //Если тайл проходимый, то добавляем его в открытый список и в зону
                    if (ns[i].tile.movementCost < Tile.walkabilityMax) {
                        openlist.Enqueue(ns[i], ++f);
                        ns[i].zone = result.data;
                        UpdateEdgeTiles(ns[i].tile);
                    }
                }
            }

            result.data.center = CalculateCenter();

            zonemap.Add(result.data, result);

            return result;
        }

        //Удаляет зону, которой принадлежит тайл
        public void RemoveZone(int x, int y)
        {
            //Определяем зону для тайла
            Zone zone = tiledata[x, y].zone;

            //Если тайл не принадлежал ни к одной зоне, то ничего не делаем
            if (zone == null || zone.id == 0) return;

            //Node_Zone node = graph.GetNode(zone);
            Node_Zone node = zonemap[zone];

            //Разрушаем связи с соседними зонами
            node.UpLink();

            //Отсоединяем тайлы от зоны
            closed.Reset(x, y);
            for (int i = closed.x_min; i < closed.x_max; ++i) {
                for (int j = closed.y_min; j < closed.y_max; ++j) {
                    if (tiledata[i, j].zone == zone) {
                        tiledata[i, j].zone = null;
                    }
                }
            }

            //возвращаем объект Node_Zone в пул  
            pool_zone.Put(node);

            zonemap.Remove(zone);
        }

        public void CalculateLinks()
        {
            tool_link.CalculateLinks();
        }

        private TileZoneData[] GetNeighbours(TileZoneData node, bool diagOK)
        {
            int x = node.tile.x;
            int y = node.tile.y;

            TileZoneData[] ns = new TileZoneData[diagOK ? 8 : 4];

            ns[0] = tiledata[x, y + 1];
            ns[1] = tiledata[x + 1, y];
            ns[2] = tiledata[x, y - 1];
            ns[3] = tiledata[x - 1, y];

            if (diagOK == false) return ns;

            ns[4] = tiledata[x + 1, y + 1];
            ns[5] = tiledata[x + 1, y - 1];
            ns[6] = tiledata[x - 1, y - 1];
            ns[7] = tiledata[x - 1, y + 1];

            return ns;
        }

        private TileZoneData GetTileNode(int x, int y)
        {
            if (x < 0) return TileZoneData.EmptyTile;
            if (y < 0) return TileZoneData.EmptyTile;
            if (x >= tiledata.GetLength(0)) return TileZoneData.EmptyTile;
            if (y >= tiledata.GetLength(1)) return TileZoneData.EmptyTile;

            return tiledata[x, y];
        }

        private void UpdateEdgeTiles(Tile t)
        {
            if (t.x > tile_e.x) tile_e = t;
            if (t.x < tile_w.x) tile_w = t;
            if (t.y > tile_n.y) tile_n = t;
            if (t.y < tile_s.y) tile_s = t;
        }

        private Tile CalculateCenter()
        {
            int x = (tile_e.x + tile_w.x) / 2;
            int y = (tile_s.x + tile_n.x) / 2;
            return tiledata[x, y].tile;
        }
    }
}