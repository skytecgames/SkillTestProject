using UnityEngine;
using System.Collections.Generic;
using Priority_Queue;

public class Find_AStar_Best
{
    ////сигнатура функции проверки тайла (для точного совпадения условий)
    //public delegate int FindCondition(Tile t);

    ////сигнатура функции для проверки тайла на условия остановки поиска
    //public delegate bool StopCondition(Tile t, int current);

    ////результат поиска
    //public Tile result = null;

    ////тайл от которого следует начинать поиск
    //private Tile tileStart;

    ////Условия, которые должны соблюдаться в искомом тайле (для поиска с точным совпадением условий)
    //private FindCondition condition;
    //private StopCondition condition_stop;

    ////Если условие превысит это пороговое значение, значит сразу можно выдать результат
    //private int edge;

    ////радиус поиска
    //private float radius;

    //public Find_AStar_Best(Tile tileStart, FindCondition condition, StopCondition condition_stop, float radius, int edge )
    //{
    //    this.tileStart = tileStart;
    //    this.condition = condition;
    //    this.radius = radius;
    //    this.edge = edge;
    //    this.condition_stop = condition_stop;
    //}

    //public void Find()
    //{
    //    World world = World.current;

    //    if (world.tileGraph == null) {
    //        world.tileGraph = new Path_TileGraph(world);
    //    }

    //    Dictionary<Tile, Path_Node<Tile>> nodes = world.tileGraph.nodes;

    //    if (nodes.ContainsKey(tileStart) == false) {
    //        world.tileGraph.AddTileToGraph(tileStart);
    //    }

    //    Path_Node<Tile> start = nodes[tileStart];

    //    List<Path_Node<Tile>> ClosedSet = new List<Path_Node<Tile>>();
    //    SimplePriorityQueue<Path_Node<Tile>> OpenSet = new SimplePriorityQueue<Path_Node<Tile>>();
    //    int current_edge = 0;

    //    Dictionary<Path_Node<Tile>, float> g_score = new Dictionary<Path_Node<Tile>, float>();
    //    foreach (Path_Node<Tile> n in nodes.Values) {
    //        g_score.Add(n, Mathf.Infinity);
    //    }
    //    g_score[start] = 0;

    //    OpenSet.Enqueue(start, -g_score[start]);

    //    while (OpenSet.Count > 0) {
    //        Path_Node<Tile> current = OpenSet.Dequeue();

    //        //Если найденный тайл лучше подходит под условия, чем предыдущая находка
    //        if (condition(current.data) > current_edge) {

    //            //запоминаем этот тайл и степень его совпадения с условиями
    //            result = current.data;
    //            current_edge = condition(current.data);

    //            //если текущий уровень совпадения превышает пороговый, значит мы нашли нужный тайл
    //            if(current_edge >= edge) return;
    //        }

    //        //Проверяем на условия досрочной остановки поиска
    //        if(condition_stop(current.data, current_edge)) {
    //            return;
    //        }

    //        ClosedSet.Add(current);

    //        foreach (Path_Edge<Tile> neighbour in current.edges) {
    //            if (ClosedSet.Contains(neighbour.node)) {
    //                continue;
    //            }

    //            //float tentative_g_score = g_score[current] + neighbour.cost * dist_bitween(current, neighbour.node);
    //            float tentative_g_score = g_score[current] + 1;

    //            if (tentative_g_score > radius) {
    //                continue;
    //            }

    //            if (OpenSet.Contains(neighbour.node) && tentative_g_score >= g_score[neighbour.node]) {
    //                continue;
    //            }

    //            g_score[neighbour.node] = tentative_g_score;

    //            if (OpenSet.Contains(neighbour.node) == false) {
    //                OpenSet.Enqueue(neighbour.node, -g_score[neighbour.node]);
    //            }
    //            else {
    //                OpenSet.UpdatePriority(neighbour.node, -g_score[neighbour.node]);
    //            }
    //        }
    //    }
    //}

    //private float cost_estimate(Path_Node<Tile> a, Path_Node<Tile> b)
    //{
    //    return (a.data.x - b.data.x) * (a.data.x - b.data.x) + (a.data.y - b.data.y) * (a.data.y - b.data.y);
    //}

    //private float dist_bitween(Path_Node<Tile> a, Path_Node<Tile> b)
    //{
    //    if (Mathf.Abs(a.data.x - b.data.x) + Mathf.Abs(a.data.y - b.data.y) == 1) return 1f;

    //    return 1.414f;
    //}
}
