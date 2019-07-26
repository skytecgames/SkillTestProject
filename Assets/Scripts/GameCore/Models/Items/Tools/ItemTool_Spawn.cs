using UnityEngine;
using System.Collections.Generic;
using Pathfinding;

public class ItemTool_Spawn
{    
    FindCondition_Spawn condition;
   
    public ItemTool_Spawn()
    {
        condition = new FindCondition_Spawn();
    }

    //Создание предмета и выбрасывание его в ближайший подходящий тайл 
    //Возвращает количество предметов, для которых не удалось найти места
    public int SpawnItem(Tile tile, string itemType, int amount)
    {
        //Нечего добавлять
        if (amount == 0) {
            Debug.LogError("SpawnItem: amount is zero");
            return 0;
        }

        //Готовим условие поиска
        condition.Init(itemType, amount, tile);

        //Получаем поисковик и инициируем поиск
        IPathfinder<Tile> path = PathfindingManager.GetConditionFinder(tile, condition);
        path.Find();

        //Размещаем предмет в полученных тайлах        
        List<Tile> found = condition.GetResult();
        if(found == null || found.Count == 0) {
            Debug.LogError("SpawnItem: can't find tile for item");
            return 0;
        }

        //Количество предметов, которое было создано
        int spawned_count = 0;

        for (int i = 0; i < found.Count; ++i) {

            //Создаем контейнер, если нужно
            if (found[i].itemContainer == null) {
                ItemManager.Instance.InitContainer(found[i]);
            }

            //Вычисляем сколько предметов мы можем разместить в контейнере
            ItemContainer target = found[i].itemContainer;
            int target_amount = Mathf.Min(amount, target.CanAdd(itemType));

            //Если 0, то пропускаем
            if (target_amount == 0) {
                Debug.LogError("SpawnItem: found container with zero capacity");
                continue;
            }

            //Размещаем предметы в контейнере
            target.AddItem(itemType, target_amount);
            amount -= target_amount;
            spawned_count += target_amount;

            //Если разместили все предметы, то выходим из цикла
            if (amount == 0) break;
        }

        return spawned_count;
    }
}
