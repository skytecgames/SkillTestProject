using System;
using System.Collections.Generic;
using UnityEngine;

namespace Pathfinding.ZStar
{
    public class Graph_ZonePath : IGraph<Zone>
    {
        //Сырые данные тайлов (тайл + зона которой он принадлежит)
        private Graph_ZoneData data;

        //Набор алгоритмов для работы с зонами
        private ZoneTool zoneTool;

        //Коллекция нодов
        private NodeCollection_Zone collection;

        //Список тайлов изменившихся с последнего обновления графа
        private List<Tile> dirtyTile;

        public Graph_ZonePath(Graph_ZoneData data)
        {
            this.data = data;            
            dirtyTile = new List<Tile>(8);
        }

        //Перестроить граф с нуля (Вызывать только после перезагрузки игры)
        public void Reset(World world)
        {
            data.zonemap = new Dictionary<Zone, Node_Zone>();
            collection = new NodeCollection_Zone(data.zonemap);
            zoneTool = new ZoneTool(data.tilemap, data.zonemap);

            //пересчитываем карту зон
            for (int x = 0; x < world.width; ++x) {
                for (int y = 0; y < world.height; ++y) {
                    //Если тайл проходимый, но не пренадлежит ни к одной зоне, то создаем новую зону
                    if (data.tilemap[x, y].tile.movementCost < Tile.walkabilityMax && data.tilemap[x, y].zone == null) {
                        zoneTool.CreateZone(x, y);                        
                    }
                }
            }
        }

        public Node_Zone GetNode(Zone zone)
        {
            if (data.zonemap.ContainsKey(zone) == false) {
                Debug.LogError("zone does not mapped");
                return null;
            }

            return data.zonemap[zone];
        }

        public IPathNodeCollection<Zone> GetNodes()
        {
            //Если у нас запрашивают коллекцию, значит готовятся к поиску пути
            UpdateGraph();

            return collection;
        }

        public void UpdateTile(Tile tile)
        {
            dirtyTile.Add(tile);            
        }

        private void UpdateGraph()
        {
            int x_min;
            int x_max;
            int y_min;
            int y_max;

            //Удаляем зоны рядом с изменившимися тайлами
            for (int i=0;i<dirtyTile.Count; ++i) {
                //берем текущий тайл
                Tile tile = dirtyTile[i];

                //Ищем прилегающие тайлы
                x_min = Mathf.Max(tile.x - 1, 0);
                x_max = Mathf.Min(tile.x + 1, data.tilemap.GetLength(0) - 1);
                y_min = Mathf.Max(tile.y - 1, 0);
                y_max = Mathf.Min(tile.y + 1, data.tilemap.GetLength(1) - 1);

                //Удаляем зоны с прилегающих тайлов
                for (int x = x_min; x < x_max + 1; ++x) {
                    for (int y = y_min; y < y_max + 1; ++y) {
                        if (data.tilemap[x, y].zone != null) {                            
                            zoneTool.RemoveZone(x, y);                            
                        }
                    }
                }
            }

            //Создаем зоны рядом с изменившимися тайлами
            for (int i = 0; i < dirtyTile.Count; ++i) {
                //берем текущий тайл
                Tile tile = dirtyTile[i];

                //Ищем прилегающие тайлы
                x_min = Mathf.Max(tile.x - 1, 0);
                x_max = Mathf.Min(tile.x + 1, data.tilemap.GetLength(0) - 1);
                y_min = Mathf.Max(tile.y - 1, 0);
                y_max = Mathf.Min(tile.y + 1, data.tilemap.GetLength(1) - 1);                

                //Создаем зоны в прилегающих тайлах
                for (int x = x_min; x < x_max + 1; ++x) {
                    for (int y = y_min; y < y_max + 1; ++y) {
                        if (data.tilemap[x, y].tile.movementCost < Tile.walkabilityMax && data.tilemap[x, y].zone == null) {
                            zoneTool.CreateZone(x, y);                            
                        }
                    }
                }
            }

            dirtyTile.Clear();

            //Обновляем группы связности зон
            zoneTool.CalculateLinks();
        }
    }
}
