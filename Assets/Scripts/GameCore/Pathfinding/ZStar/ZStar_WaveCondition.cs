using UnityEngine;
using System.Collections.Generic;

namespace Pathfinding.ZStar
{
    //Поисковый алгоритм для волнового перебора тайлов до выполнения указзаного условия
    public class ZStar_WaveCondition : IPathfinder<Tile>
    {
        //Input data
        //System.Func<Tile, bool> tileProcessAction;
        IFindCondition<Tile> condition;
        Tile startTile;
        IGraph<Tile> graph;        

        //Search data
        IPathNode<Tile> start;        
        IList<IPathNode<Tile>> CloseSet;
        Queue<IPathNode<Tile>> OpenSet;        

        public ZStar_WaveCondition()
        {
            CloseSet = new List<IPathNode<Tile>>();
            OpenSet = new Queue<IPathNode<Tile>>();
        }

        //public void Init(IGraph<Tile> graph, Tile startTile, System.Func<Tile, bool> tileProcessAction)
        public void Init(IGraph<Tile> graph, Tile startTile, IFindCondition<Tile> condition)
        {
            this.condition = condition;
            this.startTile = startTile;
            this.graph = graph;
        }

        private void Init()
        {
            //Очищаем данные поиска            
            CloseSet.Clear();
            OpenSet.Clear();                       

            //получаем коллекцию нодов
            IPathNodeCollection<Tile> collection = graph.GetNodes();

            //Получаем стартовый нод
            start = collection.GetNode(startTile);

            //Размещаем нод в открытом списке
            OpenSet.Enqueue(start);
        }

        private bool FindLoop(int count = int.MaxValue)
        {   
            while(OpenSet.Count > 0) {
                
                //Извлекаем тайл из очереди
                IPathNode<Tile> current = OpenSet.Dequeue();

                //Если уже обрабатывали этот тайл, то ничего не делаем
                if (CloseSet.Contains(current)) continue;

                //Проверяем не достигли ли мы конца поиска
                if(condition.Check(current.node)) {
                    return true;
                }

                //Добавляем в закрытый список
                CloseSet.Add(current);

                //Перебираем соседей
                IEnumerator<IPathNode<Tile>> it = current.GetNeighbours();
                while (it.MoveNext()) {

                    IPathNode<Tile> ns = it.Current;

                    //Если нод уже присутствует в отрытом или закрытом списке, то пропускаем его
                    if (CloseSet.Contains(ns)) continue;
                    if (OpenSet.Contains(ns)) continue;

                    //Добавляем в открытый список
                    OpenSet.Enqueue(ns);
                }

                //Если закончились попытки поиска, завершаем его
                if (--count <= 0) return false;
            }

            return true;
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

        public IPath<Tile> GetResult()
        {
            return null;
        }
    }
}