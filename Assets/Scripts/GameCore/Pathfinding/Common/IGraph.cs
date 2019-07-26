using UnityEngine;
using System.Collections;

namespace Pathfinding
{
    public interface IGraph<T>
    {
        //Перестроить граф с нуля
        void Reset(World world);

        // Получить слепок коллекции нодов (поиск пути может происходить в течении нескольких кадров, мы должны гарантировать
        // что во время проведения поиска мы не поменяем граф нодов, поэтому нам нужна функция получения слепка)
        IPathNodeCollection<T> GetNodes();

        //Уведомление графа об изменении тайла
        void UpdateTile(Tile t);
    }
}