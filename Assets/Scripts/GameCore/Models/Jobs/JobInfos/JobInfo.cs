using UnityEngine;
using System.Collections;

//Класс для описания, что должно быть сделано для выполнения работы, без привязки к Agent'ам
//Из экземляра этого класса и экземпляра Agent - JobPlaner должен уметь составлять класс Job
//На этом этапе формирования задачи не должны резервироваться никакие игровые ресурсы
//На этом этапе формирования задачи вычисляется ее приоритет
public abstract class JobInfo
{
    public JobType type = JobType.None;

    //Событие - на задачу назначен агент
    public System.Action<JobInfo, Agent> cbJobStart;
    //Событие - задача завершена
    public System.Action<JobInfo> cbJobComplete;
    //Событие - задача остановлена
    public System.Action<JobInfo> cbJobStop;

    public abstract bool CheckRequirements(Agent agent);

    public abstract JobNew PlanJob(Agent agent);

    protected virtual void JobStart(Agent agent)
    {
        if (cbJobStart != null) {
            cbJobStart(this, agent);
        }
    }
        
    protected virtual void OnComplete(JobNew job)
    {        
        if (cbJobComplete != null) {
            cbJobComplete(this);
        }
    }

    protected virtual void OnStop(JobNew job)
    {        
        if (cbJobStop != null) {
            cbJobStop(this);
        }
    }

    protected virtual void SetAgentToJob(Agent agent, JobNew job)
    {
        //TOTHINK: тип задачи должен передаваться не так
        job.type = type;

        //настраиваем агента для задачи
        job.agent = agent;

        //Настраиваем события для задачи
        job.cbJobComplete += OnComplete;
        job.cbJobStop += OnStop;

        agent.SetJob(job);

        //уведомляем о том, что на задачу назначен агент
        JobStart(agent);
    }
}
