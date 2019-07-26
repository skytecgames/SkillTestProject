using UnityEngine;
using System.Collections;

//менеджер для манипуляций с предметами (перемещений с места на место)
public class ItemManagerA
{
    private static ItemTool_Transfer transferTool;
    private static ItemTool_Spawn spawnTool;

    public static void Init()
    {
        transferTool = new ItemTool_Transfer();
        spawnTool = new ItemTool_Spawn();
    }

    //Найти ближайшую точку со свободным контейнером и разместить в ней указанный предмет
    //Метод позволяет добавлять в игру новые предметы
    //Предполагается, что переданный предмет не размещен еще в игре
    public static void SpawnItem(Tile tile, string itemType, int amount)
    {
        spawnTool.SpawnItem(tile, itemType, amount);
    }

    //Transfer item from source to target container. Return exact amount of transfered items
    public static int TransferItem(ItemContainer source, ItemContainer target, string itemType, int amount)
    {
        return transferTool.TransferItem(source, target, itemType, amount);
    }

    //Transfer item from source container to workpoint's material container
    public static int TransferItem(ItemContainer source, WorkPoint target, string itemType, int amount)
    {
        return transferTool.TransferItem(source, target, itemType, amount);
    }
}
