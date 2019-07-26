using UnityEngine;
using System.Collections.Generic;

public class JobInfo_Wait : JobInfo
{
    public override bool CheckRequirements(Agent agent)
    {
        //Эта задача выполнима всегда
        return true;
    }

    public override JobNew PlanJob(Agent agent)
    {        
        JobNew job = new JobNew();
        job.action_list = new List<Action>();

        //добавляем только одно действие - ждать
        job.action_list.Add(new Action_Wait(0.5f));        

        SetAgentToJob(agent, job);

        return job;
    }
}
