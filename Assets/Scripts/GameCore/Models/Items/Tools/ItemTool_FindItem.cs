using UnityEngine;
using System.Collections.Generic;
using Pathfinding.ZStar;

public class ItemTool_FindItem
{
    //Счетчик предметов
    private ItemAccountant accountant;    

    //Конструктор
    public ItemTool_FindItem(ItemAccountant accountant)
    {
        this.accountant = accountant;        
    }

    //Проверяет наличие хотя бы одного предмета указанного типа, до которого существует путь из указаного тайла
    public bool CheckAvailableItem(Tile tile_start, string itemType)
    {
        //На карте нет таких предметов
        if (accountant.GetItemCount(itemType) == 0) return false;

        //Идентификатор группы связности для стартового тайла
        int linkid = PathfindingManagerZ.GetZoneLinkId(tile_start);

        //Перебираем все контейнеры содержащие указанный тип предмета
        IEnumerator<ItemContainer> it = accountant.ContainersWithItem(itemType);
        while (it.MoveNext()) {
            Tile t = it.Current.tile;
            if (t != null && PathfindingManagerZ.GetZoneLinkId(t) == linkid) return true;
        }

        return false;
    }

    //Возвращает список контейнеров из которых можно получить указанный тип предмета, в указанном количестве
    //Соблюдается условие, что к каждому из этих контейнеров существует путь из указаного тайла
    //TO FIX: Контейнеры по возможности подбираются ближайшие
    public List<ItemContainer> GetClosestItem(Tile tile_start, string itemType, int amount)
    {
        //Если нет таких предметов, то возвращаем null
        if (accountant.GetItemCount(itemType) == 0) return null;

        //TOFIX: пул списков найденых контейнеров
        List<ItemContainer> result = new List<ItemContainer>();

        //Идентификатор группы связности для стартового тайла
        int linkid = PathfindingManagerZ.GetZoneLinkId(tile_start);

        //Перебираем контейнеры с указанным типом предмета
        IEnumerator<ItemContainer> it = accountant.ContainersWithItem(itemType);
        while(it.MoveNext()) {
            Tile t = it.Current.tile;
            if (t != null && PathfindingManagerZ.GetZoneLinkId(t) == linkid) {
                int source_amount = it.Current.HasItem(itemType);

                if(source_amount == 0) {
                    Debug.LogError("GetClosestItem: found container with zero item of type");
                    continue;
                }

                result.Add(it.Current);
                amount -= source_amount;

                if (amount <= 0) return result;
            }
        }

        return result;
    }    
}
