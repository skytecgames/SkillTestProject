using System.Collections.Generic;
using UnityEngine;

public class JobInfo_Gather : JobInfo
{
    private const ClaimType claimType = ClaimType.Gather;

    public override bool CheckRequirements(Agent agent)
    {
        //TOFIX: проверим, может ли агент выполнять такой тип работ

        //Проверим есть ли точка сбора которую может достичь агент
        if(GatherManager.GetClosestGatherPoint(agent.character.currTile) == null) {
            return false;
        }

        return true;
    }

    public override JobNew PlanJob(Agent agent)
    {
        //Создаем новый экземпляр работы
        JobNew job = new JobNew();
        job.action_list = new List<Action>();

        //Найдем ближайшую точку сбора
        WorkPoint workpoint = GatherManager.GetClosestGatherPoint(agent.character.currTile);

        //claim gather point
        TileClaimManager.Claim(workpoint.tile, claimType);

        //двигаемся к точке работы, если нужно
        if (agent.character.currTile != workpoint.tile) {
            job.action_list.Add(new Action_MoveToPoint(workpoint.tile));
        }

        //работаем на точке
        job.action_list.Add(new Action_DoWork(workpoint));

        SetAgentToJob(agent, job);

        //Связываем задачу и точку работы (отмена точки работы, вызовет приостановку задачи)
        JobWorkpointHandler handler = new JobWorkpointHandler(job, workpoint);
        JobTileClaimHandler claimHandler = new JobTileClaimHandler(job, workpoint.tile, claimType);

        return job;
    }
}
