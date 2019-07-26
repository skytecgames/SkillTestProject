using UnityEngine;
using System.Collections.Generic;

//Задача - переместить указанный тип предметов в указанную точку
//TOFIX: Задача должна формулироваться как переноска предметов в указанный склад
//       так как в конкретную точку переносить неэффективно
public class JobInfo_HaulItemToStockpile : JobInfo
{
    //TODO: эти данные не принадлежат этому классу и должны быть взяты извне
    //private ItemContainer character_container;

    //конструктор для новой задачи
    public JobInfo_HaulItemToStockpile()
    {
        //все данные для этой задачи генерируются на этапе назначения агента
    }

    //Проверить возможность выполнение задачи указанным агентом
    public override bool CheckRequirements(Agent agent)
    {
        //TOFIX: этот метод может запрашиваться очень часто и его важно максимально оптимизировать
        //       в данном случае можно добавить кеширование ответа с протуханием по времени        

        //TOFIX: это временное решение (такой способ заставит нас начать поиск по тайлам, а это слишком медленно)
        //string item_found = null;
        //ItemManager.Instance.GetClosestHaulableItem(agent.character.currTile,
        //    agent.character.itemContainer.GetVolime(), out item_found);                   

        //if (item_found != null) {            
        //    return true;
        //}

        //Проверяем наличие предметов для переноски
        if (ItemFinder.CheckHaulableItem(agent.character.currTile) == false) return false;

        return true;
    }

    //Запланировать задачу для исполнения указанным агентом
    //(вызывается непосредственно перед началом выполнения задачи)
    public override JobNew PlanJob(Agent agent)
    {
        JobNew job = new JobNew();        

        job.action_list = new List<Action>();

        //Вычисляем что, откуда и куда нести
        string itemType;
        //ItemContainer sourceContainer = ItemManager.Instance.GetClosestHaulableItem(agent.character.currTile, 
        //    agent.character.itemContainer.GetVolime(), out itemType);
        //ItemContainer destContainer = ItemManager.Instance.GetClosestStockpileForItem(sourceContainer.tile, itemType, null);

        ItemContainer sourceContainer = ItemFinder.GetClosestHaulableItem(agent.character.currTile, out itemType);
        ItemContainer destContainer = ItemFinder.GetClosestStockpileForItem(
            agent.character.currTile, itemType, sourceContainer.HasItem(itemType))[0];

        //если персонаж не стоит на месте в котором лежит предмет, то ему сначала туда нужно перейти
        if(agent.character.currTile != sourceContainer.tile) {            
            job.action_list.Add(new Action_MoveToPoint(sourceContainer.tile));
        }

        //потом нужно поднять предмет
        //TOFIX: тут нужно учитывать количество предметов, которое может перенести персонаж        
        //TOFIX: тут нужно учесть сколько предметов может принять целевой тайл
        job.action_list.Add(new Action_PickItem(sourceContainer, itemType, sourceContainer.HasItem(itemType)));

        //потом нужно пойти к целевому тайлу        
        job.action_list.Add(new Action_MoveToPoint(destContainer.tile));

        //потом нужно выложить предмет        
        job.action_list.Add(new Action_PlaceItem(destContainer, itemType, destContainer.CanAdd(itemType)));

        SetAgentToJob(agent, job);

        return job;
    }    
}
