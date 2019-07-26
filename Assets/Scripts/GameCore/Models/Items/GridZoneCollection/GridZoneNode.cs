using UnityEngine;
using System.Collections.Generic;

public class GridZoneNode
{
    //Размеры области
    private RectangleQT bounds;

    //Предметы в области
    private List<ItemContainer> items;

    public GridZoneNode(RectangleQT bounds)
    {
        this.bounds = bounds;
        items = new List<ItemContainer>();
    }

    //вставка элемента в узел
    public void Insert(ItemContainer item)
    {
        if(items.Contains(item) == false) {
            items.Add(item);
        }
    }

    //удаление элемента из узла
    public void Remove(ItemContainer item)
    {
        if(items.Contains(item) == true) {
            items.Remove(item);
        }
    }

    //получение списка элементов из узла (находящихся в указанной области)
    public List<ItemContainer> Queue(RectangleQT area)
    {
        List<ItemContainer> result = new List<ItemContainer>(items.Count);

        for(int i=0;i<items.Count;++i) {
            if (area.Contains(items[i].tile.x, items[i].tile.y)) result.Add(items[i]);
        }

        return result;
    }

    public int Count()
    {
        return items.Count;
    }
}
