using UnityEngine;
using System.Collections.Generic;

public class JobInfo_HaulItemToWorkpoint : JobInfo
{
    public string itemType;

    public WorkPoint workpoint;
    public ItemContainer sourceContainer;        

    public JobInfo_HaulItemToWorkpoint(string itemType, WorkPoint workpoint)
    {
        this.itemType = itemType;
        this.workpoint = workpoint;        
    }

    //Проверить возможность выполнение задачи указанным агентом
    public override bool CheckRequirements(Agent agent)
    {        
        //TOFIX: проверить необходимость в указанном предмете в точке работы        
        //TOFIX: проверить возможность пройти до точки работы
        //TOFIX: проверить наличие места в инвентаре персонажа 
        //      (только неснимаемые вещи, останое можно выбросить по ходу)

        //проверить наличие в мире предмета нужного типа до которого способен добраться персонаж
        if (ItemFinder.CheckAvailableItem(agent.character.currTile, itemType) == false) return false;

        //ItemContainer sc = ItemManager.Instance.GetClosestItem(agent.character.currTile, itemType,
        //        workpoint.materials.NeedMaterial(itemType) - agent.character.itemContainer.HasItem(itemType));

        //if(sc == null) {
        //    Debug.LogWarning("JobInfo_HaulItemToWorkpoint: item type not found");
        //    return false;
        //}

        //if(sc.tile == null) {
        //    Debug.LogError("JobInfo_HaulItemToWorkpoint: finded container not linked to tile");
        //    return false;
        //}

        return true;
    }

    //Запланировать задачу для исполнения указанным агентом
    //(вызывается непосредственно перед началом выполнния задачи)
    public override JobNew PlanJob(Agent agent)
    {   
        JobNew job = new JobNew();
        job.action_list = new List<Action>();

        //Расчетное количество предметов у персонажа после выполнения действий (в начале у нас то, что сейчас в инвентаре)
        int amount_at_stage = agent.character.itemContainer.HasItem(itemType);

        //TOFIX: избавить от лишних предметов на руках

        //если в инвентаре нет нужного количества предметов, то сначала двигаем к источнику
        if (agent.character.itemContainer.HasItem(itemType) < workpoint.materials.NeedMaterial(itemType)) {

            //TOFIX: нужно учесть сколько предметов персонаж может перенести
            List<ItemContainer> sourceContainers = ItemFinder.GetClosestItem(agent.character.currTile, itemType,
                workpoint.materials.NeedMaterial(itemType) - agent.character.itemContainer.HasItem(itemType));

            sourceContainer = sourceContainers[0];

            //ищем откуда мы будем брать метериалы для выполнения этой работы
            //sourceContainer = ItemManager.Instance.GetClosestItem(agent.character.currTile, itemType,
            //    workpoint.materials.NeedMaterial(itemType) - agent.character.itemContainer.HasItem(itemType));

            //если мы не в точке с предметом, переходим к ней
            if (agent.character.currTile != sourceContainer.tile) {
                job.action_list.Add(new Action_MoveToPoint(sourceContainer.tile));
            }

            //подбираем предмет в нужном количестве
            int amount_to_pick = Mathf.Min(sourceContainer.HasItem(itemType), 
                workpoint.materials.NeedMaterial(itemType) - agent.character.itemContainer.HasItem(itemType));
            job.action_list.Add(new Action_PickItem(sourceContainer, itemType, amount_to_pick));

            amount_at_stage += amount_to_pick;
        }

        //двигаемся к точке работы
        job.action_list.Add(new Action_MoveToPoint(workpoint.tile));

        //Выкладываем предмет в точку работы в нужном количестве        
        int amount_to_place = Mathf.Min(workpoint.materials.NeedMaterial(itemType), amount_at_stage);
        job.action_list.Add(new Action_PlaceMaterial(workpoint, itemType, amount_to_place));

        SetAgentToJob(agent, job);

        return job;
    }    
}
