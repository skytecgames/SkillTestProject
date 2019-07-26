using UnityEngine;
using System.Collections.Generic;

namespace Pathfinding.ZStar
{
    public class Pool_NodeZone
    {
        Stack<Node_Zone> queue = new Stack<Node_Zone>();

        //Получить новый нод зоны
        public Node_Zone Get()
        {
            if (queue.Count == 0) {

                Node_Zone n = new Node_Zone();

                n.data = new Zone();
                n.edges = new List<Node_Zone>();

                //Debug.LogFormat("ZonePool: Create [{0}]", n.data.id);

                return n;
            }

            //Debug.LogFormat("ZonePool Pick [{0}]", queue.Peek().data.id);

            return queue.Pop();
        }

        //Вурнуть нод зоны в пул
        public void Put(Node_Zone node)
        {
            //Debug.LogFormat("ZonePool: Put [{0}]", node.data.id);

            node.edges.Clear();

            queue.Push(node);
        }
    }
}