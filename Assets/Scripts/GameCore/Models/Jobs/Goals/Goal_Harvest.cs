using UnityEngine;
using System.Collections;

public class Goal_Harvest : Goal
{
    //TOFIX: для поддержки разных приоритетов тут нужно запускать несколько разных задач на сбор
    //       (добыча растений, добыча минералов, разделка животных и.т.п.)

    JobInfo_Gather gather_job;

    public override void Start()
    {
        //Эта задача должна стартовать только 1 раз
        if (gather_job != null) {
            Debug.LogError("Goal_Harvest: is started more then one times");
            return;
        }

        //Создаем задачку на переноску
        gather_job = new JobInfo_Gather();
        //устанавливаем тип задачи
        gather_job.type = JobType.Gather;

        //регистрируем задачу в JobAgent
        JobAgent.GetInstance().AddJob(gather_job);

        //больше ничего не делаем
    }
}
