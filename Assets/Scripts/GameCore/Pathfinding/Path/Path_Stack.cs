using UnityEngine;
using System.Collections.Generic;

namespace Pathfinding
{
    public class Path_Stack<T> : IPath<T>
    {
        //Пустой путь, нужен чтобы выдать в качестве результата, если путь не найден
        public static readonly Path_Stack<T> Empty = new Path_Stack<T>();

        //Данные пути
        private Stack<T> path;

        //Конструктор пустого пути
        private Path_Stack()
        {
            path = new Stack<T>();
        }

        //Конструктор пути
        public Path_Stack(Dictionary<IPathNode<T>, IPathNode<T>> cameFrom, IPathNode<T> finish)
        {
            CreatePath(cameFrom, finish);
        }

        //Построить путь по данным системы поиска пути
        private void CreatePath(Dictionary<IPathNode<T>, IPathNode<T>> cameFrom, IPathNode<T> current)
        {
            path = new Stack<T>();
            path.Push(current.node);

            while (cameFrom.ContainsKey(current)) {
                current = cameFrom[current];
                path.Push(current.node);
            }
        }

        //Следующий элемент пути
        public T GetNext()
        {
            return path.Pop();
        }

        //Длинна пути
        public int Length()
        {
            return path.Count;
        }

        //Очистка пути
        public void Clear()
        {
            path.Clear();
        }
    }
}