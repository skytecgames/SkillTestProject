using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Pathfinding.ZStar
{
    public class PathfindingManagerZ
    {
        private static Graph_ZoneData data;
        private static IGraph<Tile> graph_tile;
        private static IGraph<Zone> graph_zone;
        private static INodeConnector<Tile, Zone> nodeConnector;

        private static ZStar pathfinder;
        private static ZStar_WaveCondition pathfinder_condition;

        public static void Init(World world)
        {
            data = new Graph_ZoneData();

            graph_tile = new Graph_TilePath_ME(data);
            graph_tile.Reset(world);

            graph_zone = new Graph_ZonePath(data);
            graph_zone.Reset(world);

            nodeConnector = new NodeConnector_ZoneTile(data);

            pathfinder = new ZStar(world.width, world.height);
            pathfinder_condition = new ZStar_WaveCondition();

            world.cbTileChanged += graph_tile.UpdateTile;
            world.cbTileChanged += graph_zone.UpdateTile;
        }

        public static IPathfinder<Tile> GetPathfinder(Tile A, Tile B)
        {
            pathfinder.Init(graph_tile, graph_zone, nodeConnector, A, B);

            return pathfinder;
        }

        public static IPathfinder<Tile> GetConditionFinder(Tile A, IFindCondition<Tile> condition)
        {
            pathfinder_condition.Init(graph_tile, A, condition);

            return pathfinder_condition;
        }

        public static int GetZoneID(Tile t)
        {
            Zone z = nodeConnector.GetNode(t);
            return z == null ? 0 : z.id;
        }

        public static int GetZoneLinkId(Tile t)
        {
            Zone z = nodeConnector.GetNode(t);
            return z == null ? 0 : z.linkId;
        }

        //Возвращает true, если существует путь от точки A в точку B
        public static bool PathExist(Tile A, Tile B)
        {
            //Перестроим граф нодов, если нужно
            graph_zone.GetNodes();

            //Зона для точки B (в которую мы движемся)
            Zone zB = nodeConnector.GetNode(B);

            //Если B не принадлежит никакой зоне, то путь в нее невозможен
            if (zB == null) {                
                return false;
            }

            //Зона для точки А (из которой мы движемся)
            Zone zA = nodeConnector.GetNode(A);
            
            if(zA != null) {
                //Если точка из которой мы движемся проходима, то просто сравниваем группы связности                
                return zA.linkId == zB.linkId;
            } else {
                IPathNode<Tile> node = graph_tile.GetNodes().GetNode(A);
                IEnumerator<IPathNode<Tile>> it = node.GetNeighbours();
                while(it.MoveNext()) {
                    if(nodeConnector.GetNode(it.Current.node).linkId == zB.linkId) return true;
                }
            }

            return false;
        }        
    }
}