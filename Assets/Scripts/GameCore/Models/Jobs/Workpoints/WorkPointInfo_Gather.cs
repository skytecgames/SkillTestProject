using UnityEngine;
using System.Collections.Generic;

public class WorkPointInfo_Gather
{
    private static IDictionary<string, WorkPointInfo> collection = new Dictionary<string, WorkPointInfo>();

    public static WorkPointInfo Load(string objectType)
    {
        //TOFIX: собираемые материалы должны привязываться к идентификатору рецепта

        //Если у нас уже есть загруженные данные по таком типу объектов, то отдаем их
        if (collection.ContainsKey(objectType)) {
            return collection[objectType];
        }

        //Загружаем данные рецепта
        WorkPointRecipe_Gather recipe = RecipeManager.GetGatherRecipe(objectType);

        //Создаем настройки для точки работы
        WorkPointInfo info = new WorkPointInfo();
        recipe.GetInfo(info);
        info.OnComplete = (workpoint) => { OnComplete(recipe.items, workpoint); };        

        //сохраняем настройки в коллекции
        collection.Add(objectType, info);

        return info;
    }

    //Обработчик события - завершение производства
    private static void OnComplete(List<KeyValuePair<string, int>> items, WorkPoint workpoint)
    {
        //Спавним полученные предметы
        for (int i = 0; i < items.Count; ++i) {
            //Item item = new Item();
            //item.objectType = items[i].Key;
            //item.stackSize = items[i].Value;
            //item.maxStackSize = ItemInfo.GetMaxStackSize(item.objectType);

            //ItemManager.Instance.SpawnItem(workpoint.spawnPoint, item);
            ItemManagerA.SpawnItem(workpoint.spawnPoint, items[i].Key, items[i].Value);
        }
    }
}
