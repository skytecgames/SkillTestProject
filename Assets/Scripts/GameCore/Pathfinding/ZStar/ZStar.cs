using UnityEngine;
using System;

namespace Pathfinding.ZStar
{
    public class ZStar : IPathfinder<Tile>
    {
        //Поисковики пути для тайлов и зон
        private Pathfinder_ZStar_Zone zone_pathfinder;
        private Pathfinder_ZStar_Tile tile_pathfinder;

        private Tile tileStart;
        private Tile tileFinish;
        private IGraph<Tile> graph_tile;
        private IGraph<Zone> graph_zone;
        private INodeConnector<Tile, Zone> nodeConnector;

        //Результат поиска
        private IPath<Tile> result;

        //Алгоритм поиска
        //1. Ищем путь по зонам
        //2. Полученный путь по зонам используем для оптимизации поиска пути по тайлам
        //3. Применяем функцию выравнивания пути    

        public ZStar(int sizeX, int sizeY)
        {
            tile_pathfinder = new Pathfinder_ZStar_Tile(sizeX, sizeY);
            zone_pathfinder = new Pathfinder_ZStar_Zone();
        }

        //Инициализация поиска пути из А в В
        public void Init(IGraph<Tile> graph_tile, IGraph<Zone> graph_zone, INodeConnector<Tile, Zone> nodeConnector, Tile A, Tile B)
        {
            tileStart = A;
            tileFinish = B;

            this.graph_tile = graph_tile;
            this.graph_zone = graph_zone;
            this.nodeConnector = nodeConnector;

            result = null;
        }

        public void Find()
        {
            //Debug.LogFormat("ZStar find from [{0}:{1}] to [{2}:{3}]", tileStart.x, tileStart.y, tileFinish.x, tileFinish.y);

            //init zone pathfinder        
            zone_pathfinder.Init(graph_zone, nodeConnector, tileStart, tileFinish);

            zone_pathfinder.Find();
            IPath<Zone> path_zone = zone_pathfinder.GetResult();

            //Debug.LogFormat("ZStar: zone path length = {0}", path_zone.Length());

            //init tile path
            tile_pathfinder.Init(graph_tile, nodeConnector, path_zone, tileStart, tileFinish);

            //Execute pathfinding for tiles
            tile_pathfinder.Find();
            IPath<Tile> path_tile = tile_pathfinder.GetResult();

            //TOFIX: optimise path
            result = path_tile;
        }

        public bool FindAsync()
        {
            throw new NotImplementedException();
        }

        public IPath<Tile> GetResult()
        {
            return result;
        }
    }
}