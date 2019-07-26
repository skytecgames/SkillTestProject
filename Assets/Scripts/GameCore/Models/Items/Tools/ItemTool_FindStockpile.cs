using UnityEngine;
using System.Collections.Generic;
using Pathfinding.ZStar;

public class ItemTool_FindStockpile
{
    private StockpileOverseer stockpiles;

    //Конструктор
    public ItemTool_FindStockpile(StockpileOverseer stockpiles)
    {
        this.stockpiles = stockpiles;
    }

    public bool CheckStockpileForItem(Tile tile_start, string itemType)
    {
        int linkid = PathfindingManagerZ.GetZoneLinkId(tile_start);

        //Проверяем наличие места под указанный тип предметов на складах
        if (stockpiles.CanBeStockpiled(itemType) == 0) return false;

        //Проверяем доступность складов для указанного типа предметов
        IEnumerator<ItemContainer> it = stockpiles.ContainersWithStockpile(itemType);
        while(it.MoveNext()) {
            if (PathfindingManagerZ.GetZoneLinkId(it.Current.tile) != linkid) continue;

            return true;
        }

        return false;
    }

    public List<ItemContainer> GetClosestStockpileForItem(Tile tile_start, string itemType, int amount)
    {
        //TOFIX: itemContainer list pool
        List<ItemContainer> itemContainers = new List<ItemContainer>();        

        int linkid = PathfindingManagerZ.GetZoneLinkId(tile_start);

        //Перебираем контейнеры с складами на которых уже содержится указанный тип предметов
        IEnumerator<ItemContainer> it = stockpiles.ContainersWithStockOfItems(itemType);
        while(it.MoveNext()) {
            //Проверяем что склад в зоне доступности
            if (PathfindingManagerZ.GetZoneLinkId(it.Current.tile) != linkid) continue;

            //Добавляем найденный контейнер в список
            itemContainers.Add(it.Current);

            //Уменьшаем число единиц, которое необходимо найти
            amount -= it.Current.CanAdd(itemType);
        }

        it = stockpiles.ContainersWithEmptyStock(itemType);
        while(it.MoveNext()) {
            //Проверяем что склад в зоне доступности
            if (PathfindingManagerZ.GetZoneLinkId(it.Current.tile) != linkid) continue;

            //Добавляем найденный контейнер в список
            itemContainers.Add(it.Current);

            //Уменьшаем число единиц, которое необходимо найти
            amount -= it.Current.CanAdd(itemType);
        }

        return itemContainers;
    }
}
