using Pathfinding.ZStar;
using System.Collections.Generic;
using UnityEngine;

public class ItemTool_FindHaulable
{
    //Счетчик предметов
    private ItemAccountant accountant;
    private StockpileOverseer stockpile;
    private ItemTool_FindStockpile findStockTool;

    public ItemTool_FindHaulable(ItemAccountant accountant, StockpileOverseer stockpile, ItemTool_FindStockpile findStockTool)
    {
        this.accountant = accountant;
        this.stockpile = stockpile;
        this.findStockTool = findStockTool;
    }

    //Проверяет наличие хотя бы одного предмета требующего переноски рядом с персонажем
    public bool CheckHaulableItem(Tile tile_start)
    {
        //TOFIX: проверка наличия какого либо места на складах
        //TOFIX: проверка наличия каких либо предметов на переноску

        //Идентификатор группы связности для стартового тайла
        int linkid = PathfindingManagerZ.GetZoneLinkId(tile_start);

        //Перебираем все контейнеры содержащие указанный тип предмета
        IEnumerator<ItemContainer> it = accountant.ContainersWithHaulable();
        while (it.MoveNext()) {

            //Проверяем доступность контейнера
            Tile t = it.Current.tile;
            if (t != null && PathfindingManagerZ.GetZoneLinkId(t) != linkid) return false;

            //Проверяем доступность складов
            string itemType = stockpile.NeedHauling(it.Current);
            if (findStockTool.CheckStockpileForItem(tile_start, itemType)) return true;
        }

        return false;
    }

    public ItemContainer GetClosestHaulableItem(Tile tile_start, out string itemType)
    {        
        //Идентификатор группы связности для стартового тайла
        int linkid = PathfindingManagerZ.GetZoneLinkId(tile_start);
        itemType = null;

        //Перебираем все контейнеры содержащие указанный тип предмета
        IEnumerator<ItemContainer> it = accountant.ContainersWithHaulable();
        while (it.MoveNext()) {
            Tile t = it.Current.tile;
            if (t != null && PathfindingManagerZ.GetZoneLinkId(t) == linkid) {
                itemType = stockpile.NeedHauling(it.Current);

                //Проверяем что мы нашли ненулевой тип предметов
                if (itemType == null) {
                    Debug.LogError("GetClosestHaulableItem: item type found is null");
                    continue;
                }

                //TOFIX: если в контейнере несколько типов предметов требующих переноски, то мы обработаем только первый

                //Если под такой тип предмета нет склада, то ищем дальше
                if (stockpile.CanBeStockpiled(itemType) == 0) {
                    continue;
                }

                //Проверить наличие доступного склада (до которого возможно найти путь)                                
                IEnumerator<ItemContainer> it_stock = stockpile.ContainersWithStockpile(itemType);
                bool stock_found = false;
                while(it_stock.MoveNext()) {
                    stock_found = true;
                    break;
                }
                if (stock_found == false) continue;

                //Возвращаем найденный контейнер
                return it.Current;
            }
        }

        return null;
    }
}
