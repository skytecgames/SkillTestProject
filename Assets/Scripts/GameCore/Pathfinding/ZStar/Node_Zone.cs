using System;
using System.Collections.Generic;

namespace Pathfinding.ZStar
{
    public class Node_Zone : IPathNode<Zone>
    {
        public Zone data;
        public IList<Node_Zone> edges;

        public Zone node { get { return data; } }

        //Цену входа в нод определяем по данным зоны
        public float movementCost { get { return data.movementCost; } }

        //Список соседей
        public IEnumerator<IPathNode<Zone>> GetNeighbours()
        {
            return edges.GetEnumerator();
        }

        //Создать связь между двумя зонами (если она уже не существует)
        public void Link(Node_Zone node)
        {
            if (edges.Contains(node) == false) {
                edges.Add(node);
            }

            if (node.edges.Contains(this) == false) {
                node.edges.Add(this);
            }
        }

        //Разъединить нод со всеми его соседями
        public void UpLink()
        {
            IEnumerator<Node_Zone> it = edges.GetEnumerator();
            while (it.MoveNext()) {
                it.Current.edges.Remove(this);
            }

            edges.Clear();
        }
    }
}