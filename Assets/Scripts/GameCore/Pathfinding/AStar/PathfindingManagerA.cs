using UnityEngine;
using System.Collections;

namespace Pathfinding.AStar
{
    public class PathfindingManagerA
    {
        //Создаем конкретные классы для поисковика и графа
        private static Graph_Tile_ME graph_tile;
        private static Pathfinder_AStar pathfinder;

        public static void Init(World world)
        {
            //Создаем граф тайлов
            graph_tile = new Graph_Tile_ME();
            graph_tile.Reset(world);

            //Подписываем граф на обновление тайлов
            world.cbTileChanged += graph_tile.UpdateTile;

            //Инициализируем первый поисковик пути
            pathfinder = new Pathfinder_AStar(world.width, world.height);
        }

        //Получить готовую к использованию копию объекта для поиска пути из A в B
        public static IPathfinder<Tile> GetPathfinder(Tile A, Tile B)
        {
            //Init
            pathfinder.Init(graph_tile, A, B);

            return pathfinder;
        }
    }
}