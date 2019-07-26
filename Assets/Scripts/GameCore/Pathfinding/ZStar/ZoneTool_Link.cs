using UnityEngine;
using System.Collections.Generic;

namespace Pathfinding.ZStar
{
    //Утилита для расчета связности зон
    public class ZoneTool_Link
    {
        //входные данные
        private Dictionary<Zone, Node_Zone> zonemap;

        //Данные для работы утилиты
        Stack<IPathNode<Zone>> openlist;

        public ZoneTool_Link(Dictionary<Zone, Node_Zone> zonemap)
        {
            this.zonemap = zonemap;

            openlist = new Stack<IPathNode<Zone>>();
        }

        public void CalculateLinks()
        {   
            //Получаем итератор для зон
            IEnumerator<Zone> it = zonemap.Keys.GetEnumerator();
            int linkId = 0;

            //Удаляем идентификаторы связей
            while(it.MoveNext()) {
                it.Current.linkId = 0;
            }

            //Обновляем итератор
            it.Reset();

            //Перебираем все зоны и расчитываем группы связности
            while(it.MoveNext()) {
                if(it.Current.linkId == 0) {
                    CalculateLinkGroup(zonemap[it.Current], ++linkId);
                }
            }
        }

        private void CalculateLinkGroup(Node_Zone start, int linkId)
        {
            openlist.Clear();

            openlist.Push(start);

            while(openlist.Count > 0) {
                //берем из списка текущий элемент
                IPathNode<Zone> current = openlist.Pop();

                //Если элементу уже задана текущая группа связности, то пропускаем его
                if (current.node.linkId == linkId) continue;

                //Задаем элементу идентификатор группы связности
                current.node.linkId = linkId;

                //Перебираем соседние зоны
                IEnumerator<IPathNode<Zone>> ns = current.GetNeighbours();
                while (ns.MoveNext()) {
                    //Если уже задана группа связности, пропускаем элемент
                    if (ns.Current.node.linkId == linkId) continue;
                    //Помещаем элемент в открытый список
                    openlist.Push(ns.Current);
                }
            }
        }
    }
}