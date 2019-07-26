using UnityEngine;
using System.Collections.Generic;
using Priority_Queue;
using System.Linq;

//Алгоритм поиска пути между двумя точками
public class Path_AStar
{
    //Queue<Tile> path;

    //public Path_AStar(World world, Tile tileStart, Tile tileEnd)
    //{
    //    if(world.tileGraph == null) {
    //        world.tileGraph = new Path_TileGraph(world);
    //    }

    //    Dictionary<Tile, Path_Node<Tile>> nodes = world.tileGraph.nodes;

    //    if(nodes.ContainsKey(tileStart) == false) {            
    //        world.tileGraph.AddTileToGraph(tileStart);            
    //    }

    //    if (nodes.ContainsKey(tileEnd) == false) {
    //        Debug.LogError("Path_AStar no such end node in graph");
    //        return;
    //    }

    //    Path_Node<Tile> start = nodes[tileStart];
    //    Path_Node<Tile> end = nodes[tileEnd];

    //    List<Path_Node<Tile>> ClosedSet = new List<Path_Node<Tile>>();
    //    SimplePriorityQueue<Path_Node<Tile>> OpenSet = new SimplePriorityQueue<Path_Node<Tile>>();

    //    Dictionary<Path_Node<Tile>, Path_Node<Tile>> CameFrom = new Dictionary<Path_Node<Tile>, Path_Node<Tile>>();

    //    Dictionary<Path_Node<Tile>, float> g_score = new Dictionary<Path_Node<Tile>, float>();
    //    Dictionary<Path_Node<Tile>, float> f_score = new Dictionary<Path_Node<Tile>, float>();

    //    foreach (Path_Node<Tile> n in nodes.Values) {
    //        g_score.Add(n, Mathf.Infinity);
    //        f_score.Add(n, Mathf.Infinity);
    //    }

    //    g_score[start] = 0;               
    //    f_score[start] = cost_estimate(start, end);

    //    OpenSet.Enqueue(start, f_score[start]);

    //    while(OpenSet.Count > 0) {
    //        Path_Node<Tile> current = OpenSet.Dequeue();

    //        if(current == end) {                
    //            reconstruct_path(CameFrom, current);
    //            return;
    //        }

    //        ClosedSet.Add(current);

    //        foreach(Path_Edge<Tile> neighbour in current.edges)
    //        {
    //            if(ClosedSet.Contains(neighbour.node)) {
    //                continue;
    //            }

    //            float tentative_g_score = g_score[current] + neighbour.cost * dist_bitween(current, neighbour.node);

    //            if(OpenSet.Contains(neighbour.node) && tentative_g_score >= g_score[neighbour.node]) {
    //                continue;
    //            }

    //            CameFrom[neighbour.node] = current;
    //            g_score[neighbour.node] = tentative_g_score;
    //            f_score[neighbour.node] = g_score[neighbour.node] + cost_estimate(neighbour.node, end);

    //            if (OpenSet.Contains(neighbour.node) == false) {
    //                OpenSet.Enqueue(neighbour.node, f_score[neighbour.node]);
    //            } else {
    //                //не много ли это добавляет к вычислениям?
    //                //TOTHINK: а это правильно? мы же можем и ухудшить оценку
    //                OpenSet.UpdatePriority(neighbour.node, f_score[neighbour.node]);
    //            }
    //        }
    //    }
    //}

    //void reconstruct_path(Dictionary<Path_Node<Tile>, Path_Node<Tile>> CameFrom, Path_Node<Tile> current)
    //{
    //    path = new Queue<Tile>();
    //    path.Enqueue(current.data);

    //    while(CameFrom.ContainsKey(current))
    //    {
    //        current = CameFrom[current];
    //        path.Enqueue(current.data);
    //    }

    //    path = new Queue<Tile>(path.Reverse());
    //}

    //float cost_estimate(Path_Node<Tile> a, Path_Node<Tile> b)
    //{
    //    return (a.data.x - b.data.x) * (a.data.x - b.data.x) + (a.data.y - b.data.y) * (a.data.y - b.data.y);
    //}

    //float dist_bitween(Path_Node<Tile> a, Path_Node<Tile> b)
    //{
    //    if (Mathf.Abs(a.data.x - b.data.x) + Mathf.Abs(a.data.y - b.data.y) == 1) return 1f;

    //    return 1.414f;        
    //}

    //public Tile GetNextTile()
    //{
    //    return path.Dequeue();
    //}

    //public int Length()
    //{
    //    if(path == null)
    //    {
    //        return 0;
    //    }

    //    return path.Count;
    //}
}
