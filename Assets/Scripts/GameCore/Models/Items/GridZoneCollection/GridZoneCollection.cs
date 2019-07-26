using UnityEngine;
using System.Collections.Generic;

public class GridZoneCollection
{
    private int width;
    private int height;
    private int gridSize;

    private GridZoneNode[,] nodes;

    public GridZoneCollection(int sizeX, int sizeY, int cellSize)
    {
        width = sizeX;
        height = sizeY;
        gridSize = cellSize;

        //Создаем массив с узлами
        nodes = new GridZoneNode[
            sizeX / cellSize + ((sizeX % cellSize > 0) ? 1 : 0),
            sizeY / cellSize + ((sizeY % cellSize > 0) ? 1 : 0)];

        //инициализируем узлы
        for (int x = 0; x < nodes.GetLength(0); ++x) {
            for (int y = 0; y < nodes.GetLength(1); ++y) {
                RectangleQT bounds = new RectangleQT(x * cellSize, y * cellSize, cellSize, cellSize);
                nodes[x, y] = new GridZoneNode(bounds);
            }
        }
    }

    //вставка предмета в коллекцию
    public void Insert(ItemContainer item)
    {
        if (item.tile.x / gridSize >= nodes.GetLength(0) 
            || item.tile.y / gridSize >= nodes.GetLength(1)
            || item.tile.x < 0 || item.tile.y < 0) {
            Debug.LogError("GridZoneCollection.Insert item is out of bounds");
        }

        nodes[item.tile.x / gridSize, item.tile.y / gridSize].Insert(item);
    }

    //Удаление предмета из коллекции
    public void Remove(ItemContainer item)
    {
        if (item.tile.x / gridSize >= nodes.GetLength(0)
            || item.tile.y / gridSize >= nodes.GetLength(1)
            || item.tile.x < 0 || item.tile.y < 0) {
            Debug.LogError("GridZoneCollection.Insert item is out of bounds");
        }

        nodes[item.tile.x / gridSize, item.tile.y / gridSize].Remove(item);
    }

    //возвращает все объекты в указанной области
    public List<ItemContainer> Queue(RectangleQT area)
    {
        List<ItemContainer> result = new List<ItemContainer>();

        int x_min = area.Left / gridSize;
        int y_min = area.Bottom / gridSize;
        int x_max = area.Right / gridSize;
        int y_max = area.Top / gridSize;

        for(int x = x_min; x <= x_max; ++x) {
            for(int y = y_min; y <= y_max; ++y) {
                result.AddRange(nodes[x, y].Queue(area));
            }
        }

        return result;
    }

    //возвращает количество предметов в коллекции
    //TOTHINK: метод не эфективный, нужен ли он для чего то кроме отладки?
    public int Count()
    {
        int count = 0;

        for (int x = 0; x < nodes.GetLength(0); ++x) {
            for (int y = 0; y < nodes.GetLength(1); ++y) {
                count += nodes[x, y].Count();
            }
        }

        return count;
    }
}
