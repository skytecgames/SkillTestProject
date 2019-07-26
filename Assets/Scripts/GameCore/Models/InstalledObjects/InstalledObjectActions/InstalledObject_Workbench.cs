using UnityEngine;
using System.Collections;

public class InstalledObject_Workbench
{
    public WorkPointInfo work_info;

    public void Workbench_OnCreate(InstalledObject obj)
    {       
        //создаем новую задачу
        Goal_WorkAtWorkstation goal = new Goal_WorkAtWorkstation(JobType.Crafting);

        //создаем Workpoint для производства предмета        
        WorkPoint workpoint = new WorkPoint();
        workpoint.tile = obj.GetJobSpotTile();
        workpoint.spawnPoint = obj.GetSpawnSpotTile();
        work_info.Init(workpoint);

        //Настраиваем Goal
        goal.workpoint = workpoint;
        goal.workpoint_info = work_info;
        goal.isRepeatable = true;        

        //Подписываемся на удаление объекта
        obj.RegisterOnRemoved(goal.OnWorkstationRemove);        

        //Запуск задачи
        goal.Start();
    }

    public void Workbench_OnRemove(InstalledObject obj)
    {
        //TODO: нужно событие на разрушение Workbench
        //      в этом событии нужно стопнуть Goal
    }
}
