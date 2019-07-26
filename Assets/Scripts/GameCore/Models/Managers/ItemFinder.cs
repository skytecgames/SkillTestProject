using System.Collections.Generic;
using UnityEngine;

public class ItemFinder
{
    //Учет предметов и складов
    private static ItemAccountant accountant;
    private static StockpileOverseer stockpile;

    //Инструменты для поиска
    private static ItemTool_FindItem findItemTool;
    private static ItemTool_FindHaulable findHaulableTool;
    private static ItemTool_FindStockpile findStockpileTool;

    public static void Init(World world)
    {
        stockpile = new StockpileOverseer();
        accountant = new ItemAccountant(world, stockpile);

        findItemTool = new ItemTool_FindItem(accountant);
        findStockpileTool = new ItemTool_FindStockpile(stockpile);
        findHaulableTool = new ItemTool_FindHaulable(accountant, stockpile, findStockpileTool);
    }

    //Проверка на наличие предмета указанного типа, до которого можно найти путь от указанного тайла.
    public static bool CheckAvailableItem(Tile tile_start, string itemType)
    {
        return findItemTool.CheckAvailableItem(tile_start, itemType);
    }

    //Поиск ближайших контейнеров в которых можно разместить указанное количество предметов заданного типа
    public static List<ItemContainer> GetClosestItem(Tile tile_start, string itemType, int amount)
    {
        return findItemTool.GetClosestItem(tile_start, itemType, amount);
    }

    //Проверяет наличие предметов требующих перености
    public static bool CheckHaulableItem(Tile tile_start)
    {
        return findHaulableTool.CheckHaulableItem(tile_start);
    }

    //Возвращает контейнер и тип предмета в нем требующий переноски
    //Гарантирует наличие подходящего склада для найденного предмета
    public static ItemContainer GetClosestHaulableItem(Tile tile_start, out string itemType)
    {
        return findHaulableTool.GetClosestHaulableItem(tile_start, out itemType);
    }

    //Возвращает список контейнеров в которых можно разместить предметы типа itemType в количестве amount
    //или хотя бы часть предметов указанного типа
    public static List<ItemContainer> GetClosestStockpileForItem(Tile tile_start, string itemType, int amount)
    {
        return findStockpileTool.GetClosestStockpileForItem(tile_start, itemType, amount);
    }

    public static void AddStockpile(ItemContainer cont)
    {
        stockpile.Register(cont);
    }

    public static void RemoveStockpile(ItemContainer cont)
    {
        stockpile.UnRegister(cont);
    }
}