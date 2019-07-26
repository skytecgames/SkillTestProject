using UnityEngine;
using System.Collections.Generic;

//Класс для управления состоянием предметов в мире
public class ItemManager
{
    #region Instance

    private static ItemManager instance = null;
    public static ItemManager Instance
    {
        get
        {
            if(instance  == null) {
                instance = new ItemManager();
            }

            return instance;
        }
    }

    #endregion    

    //Все контенеры в игре
    public List<ItemContainer> itemContainers = new List<ItemContainer>();    

    //Инициализирует контейнер на тайле
    public void InitContainer(Tile tile)
    {
        if (tile.itemContainer == null) {
            tile.itemContainer = new ItemContainer();

            //добавляем контейнер в список всех контейнеров игры
            itemContainers.Add(tile.itemContainer);

            //прикрепляем контейнер к тайлу
            tile.itemContainer.tile = tile;

            if (World.current.cbItemContainerCreated != null) {
                World.current.cbItemContainerCreated(tile.itemContainer);
            }
        }
    }
}
