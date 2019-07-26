using UnityEngine;
using System.Collections;

public class InstalledObject_Stockpile
{
    public static void Stockpile_OnCreate(InstalledObject obj)
    {
        //Инициализируем контейнер для предмета на тайле
        ItemManager.Instance.InitContainer(obj.tile);

        //ItemManager.Instance.stockpileManager.AddStockpile(obj.tile.itemContainer);
        ItemFinder.AddStockpile(obj.tile.itemContainer);
        obj.RegisterOnRemoved(Stockpile_OnRemove);
    }

    public static void Stockpile_OnRemove(InstalledObject obj)
    {
        obj.UnRegisterOnRemoved(Stockpile_OnRemove);
        //ItemManager.Instance.stockpileManager.RemoveStockpile(obj.tile.itemContainer);
        ItemFinder.RemoveStockpile(obj.tile.itemContainer);
    }
}
