using UnityEngine;
using System.Collections.Generic;

namespace Pathfinding.ZStar
{
    public class Pool_NodeTile
    {
        private Stack<Node_Tile> stack = new Stack<Node_Tile>();

        public Node_Tile Get(Tile t)
        {
            if (stack.Count == 0) {
                return new Node_Tile(t);
            }

            stack.Peek().Reset(t);
            return stack.Pop();
        }

        public void Put(Node_Tile node)
        {
            stack.Push(node);
        }
    }
}