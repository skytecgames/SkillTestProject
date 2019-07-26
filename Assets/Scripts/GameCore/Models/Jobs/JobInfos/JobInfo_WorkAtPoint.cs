using UnityEngine;
using System.Collections.Generic;
using Pathfinding.ZStar;

public class JobInfo_WorkAtPoint : JobInfo
{
    public WorkPoint workpoint;

    public JobInfo_WorkAtPoint(WorkPoint workpoint)
    {
        this.workpoint = workpoint;
    }

    public override bool CheckRequirements(Agent agent)
    {
        //TODO: проверим наличие всех материалов на точке работы
        
        //проверить, что агент может достич точки работы
        //if(PathfindingManagerZ.GetZoneLinkId(workpoint.tile) != PathfindingManagerZ.GetZoneLinkId(agent.character.currTile)) {
        if(PathfindingManagerZ.PathExist(agent.character.currTile, workpoint.tile) == false) { 
            return false;
        }

        //TOFIX: проверим что агент может выполнять такой тип работы        

        return true;
    }

    public override JobNew PlanJob(Agent agent)
    {
        JobNew job = new JobNew();
        job.action_list = new List<Action>();        

        //двигаемся к точке работы, если нужно
        if(agent.character.currTile != workpoint.tile) {
            job.action_list.Add(new Action_MoveToPoint(workpoint.tile));
        }

        //работаем на точке
        job.action_list.Add(new Action_DoWork(workpoint));

        SetAgentToJob(agent, job);        

        return job;
    }

    private void OnWorkpointCancel(JobNew job, WorkPoint point)
    {
        job.Cancel();
    }
}
