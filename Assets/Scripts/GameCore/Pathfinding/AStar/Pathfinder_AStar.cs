using UnityEngine;
using System.Collections.Generic;
using Priority_Queue;

namespace Pathfinding.AStar
{
    public class Pathfinder_AStar : IPathfinder<Tile>
    {
        //Входные данные
        IGraph<Tile> graph;
        Tile tileStart;
        Tile tileEnd;

        //Данные поиска (Подготавливаются при вызове Init)
        IPathNode<Tile> end;
        List<IPathNode<Tile>> ClosedSet;
        SimplePriorityQueue<IPathNode<Tile>> OpenSet;
        Dictionary<IPathNode<Tile>, IPathNode<Tile>> CameFrom;
        float[,] g_score;
        float[,] f_score;

        //результат поиска
        private IPath<Tile> result = null;

        //Конструктор для базового поиска из точки в точку
        public Pathfinder_AStar(int sizeX, int sizeY)
        {
            g_score = new float[sizeX, sizeY];
            f_score = new float[sizeX, sizeY];

            ClosedSet = new List<IPathNode<Tile>>();
            OpenSet = new SimplePriorityQueue<IPathNode<Tile>>();
            CameFrom = new Dictionary<IPathNode<Tile>, IPathNode<Tile>>();
        }

        public void Find()
        {
            Init();

            FindLoop();
        }

        public bool FindAsync()
        {
            throw new System.NotImplementedException();
        }

        public void Init(IGraph<Tile> graph, Tile tileStart, Tile tileEnd)
        {
            this.graph = graph;
            this.tileStart = tileStart;
            this.tileEnd = tileEnd;

            result = null;
        }

        private bool Init()
        {
            //Нет графа для поиска
            if (graph == null) {
                Debug.Log("AStar.Init graph is null");
                return false;
            }

            IPathNodeCollection<Tile> nodes = graph.GetNodes();

            //Проверим что в конечный нод можно попасть
            if (nodes.GetNode(tileEnd).movementCost >= Tile.walkabilityMax) {
                return false;
            }

            IPathNode<Tile> start = nodes.GetNode(tileStart);
            end = nodes.GetNode(tileEnd);

            ClosedSet.Clear();
            OpenSet.Clear();
            CameFrom.Clear();

            //Обновляем таблицы g_score и f_score
            for (int x = 0; x < g_score.GetLength(0); ++x) {
                for (int y = 0; y < g_score.GetLength(1); ++y) {
                    g_score[x, y] = Mathf.Infinity;
                    f_score[x, y] = Mathf.Infinity;
                }
            }

            //Добавляем стартовую точку в f_score и g_score
            g_score[start.node.x, start.node.y] = 0;
            f_score[start.node.x, start.node.y] = cost_estimate(start, end);

            OpenSet.Enqueue(start, f_score[start.node.x, start.node.y]);

            return true;
        }

        //Цикл поиска
        private bool FindLoop(int count = int.MaxValue)
        {
            while (OpenSet.Count > 0) {

                //Получаем следующий тайл
                IPathNode<Tile> current = OpenSet.Dequeue();

                //Если нашли нужный тайл, то завершаем поиск
                if (current == end) {
                    result = new Path_Stack<Tile>(CameFrom, end);
                    return true;
                }

                ClosedSet.Add(current);

                //перебираем соседей
                IEnumerator<IPathNode<Tile>> it = current.GetNeighbours();
                while (it.MoveNext()) {

                    IPathNode<Tile> ns = it.Current;

                    //Если нод в закрытом списке, то идем дальше
                    if (ClosedSet.Contains(ns)) {
                        continue;
                    }

                    float tentative_g_score
                        = g_score[current.node.x, current.node.y] + ns.movementCost * dist_bitween(current, ns);

                    if (OpenSet.Contains(ns) && tentative_g_score >= g_score[ns.node.x, ns.node.y]) {
                        continue;
                    }

                    CameFrom[ns] = current;
                    g_score[ns.node.x, ns.node.y] = tentative_g_score;
                    f_score[ns.node.x, ns.node.y] = g_score[ns.node.x, ns.node.y] + cost_estimate(ns, end);

                    if (OpenSet.Contains(ns) == false) {
                        OpenSet.Enqueue(ns, f_score[ns.node.x, ns.node.y]);
                    }
                    else {
                        //не много ли это добавляет к вычислениям?                    
                        OpenSet.UpdatePriority(ns, f_score[ns.node.x, ns.node.y]);
                    }
                }

                if (--count <= 0) return false;
            }

            return true;
        }

        public IPath<Tile> GetResult()
        {
            return result;
        }

        float cost_estimate(IPathNode<Tile> a, IPathNode<Tile> b)
        {
            return (a.node.x - b.node.x) * (a.node.x - b.node.x) + (a.node.y - b.node.y) * (a.node.y - b.node.y);
        }

        float dist_bitween(IPathNode<Tile> a, IPathNode<Tile> b)
        {
            if (Mathf.Abs(a.node.x - b.node.x) + Mathf.Abs(a.node.y - b.node.y) == 1) return 1f;

            return 1.414f;
        }
    }
}