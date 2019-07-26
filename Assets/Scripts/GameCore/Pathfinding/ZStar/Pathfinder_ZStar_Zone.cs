using UnityEngine;
using System.Collections.Generic;
using System;
using Priority_Queue;

namespace Pathfinding.ZStar
{
    public class Pathfinder_ZStar_Zone : IPathfinder<Zone>
    {
        //Входные данные
        private IGraph<Zone> graph;
        private INodeConnector<Tile, Zone> nodeConnector;
        private Zone zoneStart;
        private Zone zoneFinish;
        private Tile tileStart;

        //Пустая зона (нужна на случай, если в стартовой точке нет зоны)
        private Node_Zone emptyZone = new Node_Zone();

        //стартовая зона
        private IPathNode<Zone> start;

        //цель поиска
        private IPathNode<Zone> finish;

        //Коллекция нодов по которой будет вестись поиск
        IPathNodeCollection<Zone> nodes;

        //Открытый список нодов
        private SimplePriorityQueue<IPathNode<Zone>> OpenSet;

        //Закрытый список
        private IList<IPathNode<Zone>> ClosedSet;

        //карта пути (с какого нода мы пришли)
        Dictionary<IPathNode<Zone>, IPathNode<Zone>> CameFrom;

        //приоритет (считай порядковый номер) нода при извлечении из очереди
        private float f = 0;

        //результат поиска
        private IPath<Zone> result = null;

        public Pathfinder_ZStar_Zone()
        {
            //Создаем открытый список нодов
            OpenSet = new SimplePriorityQueue<IPathNode<Zone>>();

            //Создаем закрытый список
            ClosedSet = new List<IPathNode<Zone>>();

            //Из какого в какой нод мы шли при поиске пути
            CameFrom = new Dictionary<IPathNode<Zone>, IPathNode<Zone>>();
        }

        public void Init(IGraph<Zone> graph, INodeConnector<Tile, Zone> nodeConnector, Tile tileStart, Tile tileFinish)
        {
            this.graph = graph;
            this.nodeConnector = nodeConnector;
            this.tileStart = tileStart;

            //Запрашиваем коллекцию нодов, мы делаем это здесь чтобы графф обновился перед обращением к
            //конектору нодов
            nodes = graph.GetNodes();

            zoneStart = nodeConnector.GetNode(tileStart);
            zoneFinish = nodeConnector.GetNode(tileFinish);
        }

        public void Find()
        {
            Init();

            //Если целевой нод недоступен, то поиск пути невозможен
            if (zoneFinish == null) return;

            FindLoop();
        }

        public bool FindAsync()
        {
            throw new NotImplementedException();
        }

        private void Init()
        {
            //Если целевой нод недоступен, то поис пути невозможен
            if (zoneFinish == null) return;

            //Получаем актуальную на время начала поиска копию коллекции нодов
            //Гарантируется, что коллекция не поменяется за время поиска
            //IPathNodeCollection<Zone> nodes = graph.GetNodes();

            //очищаем списки данных
            OpenSet.Clear();            
            ClosedSet.Clear();            
            CameFrom.Clear();

            //Получаем статовый нод
            if (zoneStart != null) {
                //Если стартовый тайл доступен, то просто берем его из коллекции
                start = nodes.GetNode(zoneStart);
            }
            else {

                //TODO: Тут нужно учитывать связность зон, если к стартовому тайлу прилегает несколько зон
                //      и между этими зонами нет пути, то может получиться так, что персонаж выберет ту,
                //      из которой нельзя прийти к финишу

                //Если стартовый тайл недоступен, то добавляем в качестве стартовой пустую зону
                //и ищем зоны прилегающие к стартовому тайлу
                Tile[] ns = tileStart.GetNeighbours(true);                
                for(int i = 0; i < ns.Length; ++i) {
                    Zone z = nodeConnector.GetNode(ns[i]);
                    if (z != null && z.linkId == zoneFinish.linkId) {
                        start = nodes.GetNode(z);                        
                        CameFrom[emptyZone] = start;
                        break;
                    }
                }
            }

            //Получаем целевой нод
            finish = nodes.GetNode(zoneFinish);

            OpenSet.Enqueue(start, f);
            ClosedSet.Add(start);
        }

        private bool FindLoop(int count = int.MaxValue)
        {
            while (OpenSet.Count > 0) {
                IPathNode<Zone> current = OpenSet.Dequeue();

                //если мы завершили поиск
                if (current == finish) {
                    result = new Path_Stack<Zone>(CameFrom, finish);
                    return true;
                }

                //перебираем соседей
                IEnumerator<IPathNode<Zone>> it = current.GetNeighbours();
                while (it.MoveNext()) {
                    IPathNode<Zone> ns = it.Current;

                    //Если в закрытом списке, идем дальше
                    if (ClosedSet.Contains(ns)) continue;

                    //Добавляем в закрытый список
                    ClosedSet.Add(ns);

                    //Помечаем откуда мы пришли в этот нод
                    CameFrom[ns] = current;

                    //Добавляем в открытый список
                    OpenSet.Enqueue(ns, ++f);
                }

                if (--count <= 0) return false;
            }

            //Не нашли путь
            return true;
        }

        public IPath<Zone> GetResult()
        {
            if (result != null) return result;

            //вернуть пустой путь
            return Path_Stack<Zone>.Empty;
        }
    }
}