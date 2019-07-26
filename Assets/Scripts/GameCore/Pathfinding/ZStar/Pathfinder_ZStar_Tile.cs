using UnityEngine;
using System.Collections.Generic;
using System;
using Priority_Queue;

namespace Pathfinding.ZStar
{
    public class Pathfinder_ZStar_Tile : IPathfinder<Tile>
    {
        //входные данные
        private IGraph<Tile> graph;
        private IPath<Zone> zonePath;
        private INodeConnector<Tile, Zone> nodeConnector;
        private Tile tileStart;
        private Tile tileFinish;

        //стартовая зона
        private IPathNode<Tile> start;

        //цель поиска
        private IPathNode<Tile> finish;

        //Открытый список нодов
        private SimplePriorityQueue<IPathNode<Tile>> OpenSet;

        //Закрытый список
        private IList<IPathNode<Tile>> ClosedSet;

        //карта пути (с какого нода мы пришли)
        Dictionary<IPathNode<Tile>, IPathNode<Tile>> CameFrom;

        //Оценочные значения для пути
        float[,] g_score;
        float[,] f_score;

        //Следующая и текущая зона зона
        private Zone nextZone;
        private Zone currentZone;

        //Тайл в направлении которого мы ведем поиск
        private Tile directionTile;

        //результат поиска    
        private IPath<Tile> result = null;

        public Pathfinder_ZStar_Tile(int sizeX, int sizeY)
        {
            //Создаем открытый список нодов
            OpenSet = new SimplePriorityQueue<IPathNode<Tile>>();

            //Создаем закрытый список
            ClosedSet = new List<IPathNode<Tile>>();

            //Из какого в какой нод мы шли при поиске пути
            CameFrom = new Dictionary<IPathNode<Tile>, IPathNode<Tile>>();

            g_score = new float[sizeX, sizeY];
            f_score = new float[sizeX, sizeY];
        }

        //Инициализация поиска из точки в точку
        public void Init(IGraph<Tile> graph, INodeConnector<Tile, Zone> nodeConnector, IPath<Zone> zonePath, Tile tileStart, Tile tileFinish)
        {
            this.graph = graph;
            this.zonePath = zonePath;
            this.tileStart = tileStart;
            this.tileFinish = tileFinish;
            this.nodeConnector = nodeConnector;

            //TOFIX: нужно сделать путь очищаемым, чтобы не пересоздавать память
            result = null;
        }

        private void Init()
        {
            //Получаем актуальную на время начала поиска копию коллекции нодов
            //Гарантируется, что коллекция не поменяется за время поиска
            IPathNodeCollection<Tile> nodes = graph.GetNodes();

            //Очищаем данные поиска
            OpenSet.Clear();
            ClosedSet.Clear();
            CameFrom.Clear();

            //Получаем статовый и целевой ноды
            start = nodes.GetNode(tileStart);
            finish = nodes.GetNode(tileFinish);

            //Добавляем стартовую точку в f_score и g_score
            g_score[start.node.x, start.node.y] = 0;
            f_score[start.node.x, start.node.y] = cost_estimate(tileStart, tileFinish);

            //Put start node to OpenSet
            OpenSet.Enqueue(start, f_score[start.node.x, start.node.y]);            

            //берем стартовую зону и целевую зону
            currentZone = zonePath.GetNext();
            nextZone = (zonePath.Length() > 0) ? zonePath.GetNext() : null;

            //Обновляем тайл в направлении которого начнем поиск
            UpdateDirectionTile();
        }

        public void Find()
        {
            if (zonePath == null || zonePath.Length() == 0) return;

            Init();

            FindLoop();
        }

        public bool FindAsync()
        {
            throw new NotImplementedException();
        }

        private bool FindLoop(int count = int.MaxValue)
        {
            //Debug.LogFormat("ZStar: tile pathfind loop start [OpenSet = {0}]", OpenSet.Count);

            while (OpenSet.Count > 0) {
                //Получаем следующий тайл
                IPathNode<Tile> current = OpenSet.Dequeue();

                //Если нашли нужный тайл, то завершаем поиск
                if (current == finish) {
                    result = new Path_Stack<Tile>(CameFrom, finish);
                    return true;
                }

                //Если тайл уже в закрытом списке, пропускаем его
                if (ClosedSet.Contains(current)) {
                    continue;
                }

                ClosedSet.Add(current);

                //Debug.LogFormat("ZStar check tile [{0},{1}]", current.node.x, current.node.y);

                //перебираем соседей
                IEnumerator<IPathNode<Tile>> it = current.GetNeighbours();
                while (it.MoveNext()) {

                    IPathNode<Tile> ns = it.Current;

                    //Если нод в закрытом списке, то идем дальше
                    if (ClosedSet.Contains(ns)) {
                        continue;
                    }

                    //Определяем зону в которой находится тайл                
                    Zone ns_zone = nodeConnector.GetNode(ns.node);

                    //TOTHINK: можно при поиске пути не отбрасывать тайлы которые не принадлежат следующей или текущей зоне,
                    //         но при этом соеденены со следующей зоной

                    //Если не из текущей или следующей зоны, то отбрасываем этот нод
                    if (ns_zone != currentZone && ns_zone != nextZone) {

                        //TOTHINK: правильно ли добавлять нод в закрытый список??
                        //TODO: тут есть проблема если текущий тайл это угол сектора и при этом противоположный угол, это единственный проход
                        //      может получиться что путь найдется через прилегающий боковой сектор, тогда помещение тайла в закрытый список
                        //      станет проблемой
                        ClosedSet.Add(ns);
                        continue;
                    }

                    //Если находится в следующей зоне, то меняем зону
                    if (nextZone != null && ns_zone == nextZone) {
                        //Очищаем открытый список
                        OpenSet.Clear();

                        //Переключаем зону
                        currentZone = nextZone;
                        nextZone = zonePath.Length() > 0 ? zonePath.GetNext() : null;

                        //Переключаем тайл в направлении которого мы ведем поиск
                        UpdateDirectionTile();

                        //Debug.Log("ZStar switch to next zone");
                    }
                    
                    float tentative_g_score
                        = g_score[current.node.x, current.node.y] + ns.movementCost * dist_bitween(current, ns);

                    if (OpenSet.Contains(ns) && tentative_g_score >= g_score[ns.node.x, ns.node.y]) {
                        continue;
                    }

                    CameFrom[ns] = current;
                    g_score[ns.node.x, ns.node.y] = tentative_g_score;
                    f_score[ns.node.x, ns.node.y] = g_score[ns.node.x, ns.node.y] + cost_estimate(ns.node, tileFinish);

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

        private void UpdateDirectionTile()
        {
            if (nextZone != null) {
                //центр зоны
                directionTile = nextZone.center;
                return;
            }

            directionTile = tileFinish;
        }

        float cost_estimate(Tile a, Tile b)
        {
            if (a == null) Debug.LogError("A is null");
            if (b == null) Debug.LogError("B is null");

            return (a.x - b.x) * (a.x - b.x) + (a.y - b.y) * (a.y - b.y);
        }

        float dist_bitween(IPathNode<Tile> a, IPathNode<Tile> b)
        {
            if (Mathf.Abs(a.node.x - b.node.x) + Mathf.Abs(a.node.y - b.node.y) == 1) return 1f;

            return 1.414f;
        }

        public IPath<Tile> GetResult()
        {
            return result;
        }
    }
}