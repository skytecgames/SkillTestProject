using UnityEngine;
using System.Collections.Generic;
using System.Text;

//Класс для управления производством на станке (предназначенном для одного персонажа)
public class Goal_WorkAtWorkstation : Goal
{   
    //точка работы для станка
    public WorkPoint workpoint;

    //Настройки для точки работы
    //TOFIX: в последствии тут должен быть список задач про последовательного выполнения
    public WorkPointInfo workpoint_info;

    //TOFIX: настройки задачи
    public bool isRepeatable = false;

    //тип задачи (строительство, работа на станке, итп)
    private JobType workType = JobType.None;

    //список всех задач
    private List<JobInfo> jobs;

    //список всех активных задач
    private List<JobNew> jobs_active;

    //список задач на переноску материалов (с ключом по материалу)
    private Dictionary<string, JobInfo> jobs_hauling;

    //задача на работу
    private JobInfo job_work;

    //статусы всех задач
    private Dictionary<JobInfo, JobStatus> jobStatuses;    

    public Goal_WorkAtWorkstation(JobType type)
    {
        workType = type;
    }

    public override void Start()
    {        
        if(workpoint == null) {
            Debug.LogError("Goal_WorkAtWorkstation: start without workpoint");
            return;
        }        

        //настраиваем точку работы
        PrepareNextWork();

        jobs = new List<JobInfo>();
        jobs_hauling = new Dictionary<string, JobInfo>();
        jobStatuses = new Dictionary<JobInfo, JobStatus>();
        jobs_active = new List<JobNew>();

        //Создаем задачи на переноску материалов
        List<string> materialTypes = workpoint.materials.GetMaterialTypes();
        foreach(string mat in materialTypes) {    
            //Создаем и настраиваем задачу
            JobInfo_HaulItemToWorkpoint job_mat = new JobInfo_HaulItemToWorkpoint(mat, workpoint);
            job_mat.cbJobComplete += OnHaulingJobComplete;
            job_mat.cbJobStart += OnHaulingJobStart;
            job_mat.cbJobStop += OnHaulingJobStop;

            //Устанавливаем тип задачи
            job_mat.type = workType;

            //Добавляем задачу во все списки
            jobs.Add(job_mat);           
            jobs_hauling.Add(mat, job_mat);            
            ChangeJobStatus(job_mat, JobStatus.Status.None);
        }

        //Создаем и настраиваем задачу на работу на точке
        job_work = new JobInfo_WorkAtPoint(workpoint);
        job_work.type = workType;
        job_work.cbJobComplete += OnWorkJobComplete;
        job_work.cbJobStart += OnWorkJobStart;
        job_work.cbJobStop += OnWorkJobStop;

        jobs.Add(job_work);
        ChangeJobStatus(job_work, JobStatus.Status.None);

        //Обновляем задачу
        Update();
    }

    public override void Pause(bool state)
    {
        //TODO: пауза для работы на точке в Goal_WorkAtWorkstation
    }

    //остановка для работы на точке в Goal_WorkAtWorkstation
    public override void Stop()
    {        
        Debug.Log("Goal_WorkAtWorkstation.Stop");

        //меняем статус всем задачаем на None (чтобы убрать их из JobManager)
        for(int i=0; i<jobs.Count; ++i) {

            //отписываем задачу по переноске материалов от обработчиков событий
            if(jobs[i] is JobInfo_HaulItemToStockpile) {
                jobs[i].cbJobComplete -= OnHaulingJobComplete;
                jobs[i].cbJobStart -= OnHaulingJobStart;
                jobs[i].cbJobStop -= OnHaulingJobStop;
            }

            //отписываем задачу по работе на точке от обработчиков событий
            if(jobs[i] is JobInfo_WorkAtPoint) {
                jobs[i].cbJobComplete -= OnWorkJobComplete;
                jobs[i].cbJobStart -= OnWorkJobStart;
                jobs[i].cbJobStop -= OnWorkJobStop;
            }

            //меняем статус всем задачам на None (убираем их из JobAgent)
            ChangeJobStatus(jobs[i], JobStatus.Status.None);
            
            // остановка всех выполняемых в данный момент задач
            // для этого в событиях JobStart нам нужно запоминать агентов
            // и подписываться на событие jobStop, jobComplete для удаления агентов
            for(int j = 0; j < jobs_active.Count; ++j) {
                jobs_active[j].Cancel();
            }
        }

        //удаляем точку работы
        workpoint.Cancel();
        workpoint = null;

        //открепляем описание процесса производства
        workpoint_info = null;

        //Очищаем все данные по задачам
        jobs.Clear();
        jobs_hauling.Clear();
        job_work = null;
        jobStatuses.Clear();
        jobs_active.Clear();
    }

    //Событие: удалили объект к которому привязано производство
    public void OnWorkstationRemove(InstalledObject obj)
    {
        //Останавливаем задачу
        Stop();

        //Отписываемся от события
        obj.UnRegisterOnRemoved(OnWorkstationRemove);
    }

    private void PrepareNextWork()
    {
        workpoint_info.Reset(workpoint);
    }

    private void OnHaulingJobComplete(JobInfo job)
    {        
        //выясняем какой материал доставлялся посредством этой задачи
        //если нужна в этом материале иссякла, то помечаем задачу как завершенную
        //в противном случае меняем ее статус на ожидание агента

        JobInfo_HaulItemToWorkpoint j = job as JobInfo_HaulItemToWorkpoint;
        
        if(j == null) {
            Debug.LogError("callback handler for haul material get job of another type");
            return;
        }

        if(workpoint == null) {
            Debug.LogError("workpoint is null. Most likely we deconstruct workstation");
            return;
        }

        if(workpoint.materials.NeedMaterial(j.itemType) > 0) {
            ChangeJobStatus(job, JobStatus.Status.Wait);
        } else {
            ChangeJobStatus(job, JobStatus.Status.Complete);
            Update();
        }
    }

    private void OnHaulingJobStart(JobInfo job, Agent agent)
    {
        //меняем статус задачи на выполнение в данный момент
        //(это удалит задачу из очереди на ожидание агента)

        ChangeJobStatus(job, JobStatus.Status.Running);

        //Запоминаем текущую задачу, как активную и подписываемся на завершающие ее события
        OnJobStart(agent.currJob);
    }    

    private void OnHaulingJobStop(JobInfo job)
    {
        //тут нужно сделать тоже самое что и при нормальном завершении задачи

        OnHaulingJobComplete(job);
    }

    private void OnWorkJobComplete(JobInfo job)
    {        
        //TOTHINK: возможно тут нужна какая то проверка - действительно ли задача была завершена

        //меняем статус задачи на завершенный
        //потом нужно вызвать функцию которая примет решение о продолжении работы или ее прекращении
        //TOFIX: нужна настройки для этой задачи

        ChangeJobStatus(job, JobStatus.Status.Complete);
                
        if(isRepeatable) {

            foreach(JobInfo j in jobs) {
                ChangeJobStatus(j, JobStatus.Status.None);
            }

            PrepareNextWork();
            Update();
        }        
    }

    private void OnWorkJobStart(JobInfo job, Agent agent)
    {
        ChangeJobStatus(job, JobStatus.Status.Running);

        //Запоминаем текущую задачу, как активную и подписываемся на завершающие ее события
        OnJobStart(agent.currJob);
    }

    private void OnWorkJobStop(JobInfo job)
    {
        //если работа была остановлено, меняем ее статус на ожидание агента

        if(workpoint.workAmount > 0) {
            ChangeJobStatus(job, JobStatus.Status.Wait);
        } else {
            Debug.LogError("callback handler (goal_work): work stop after completion");
            OnWorkJobComplete(job);
        }
    }    

    private void OnJobStart(JobNew job)
    {        
        //Запоминаем текущую задачу, как активную и подписываемся на завершающие ее события
        jobs_active.Add(job);
        job.cbJobComplete += OnJobComplete;
        job.cbJobStop += OnJobComplete;        
    }

    //Обработчик события на завершение активной задачи
    private void OnJobComplete(JobNew job)
    {
        if (jobs_active.Contains(job)) {

            //отписываемся от обработчиков событий
            job.cbJobComplete -= OnJobComplete;
            job.cbJobStop -= OnJobComplete;

            //удаляем задачу из списка активных
            jobs_active.Remove(job);
        }        
    }

    //Задачи на таком уровне не обновляются по времени
    //их обновление происходит при изменении статуса принадлежащих им заданий
    //Обновляет статус задач требующихся для выполнения цели. Тут только проверка на очевидные проблемы
    //и запуск задач, остановка задач осуществляется в обработчиках событий
    private void Update()
    {        
        //если все материалы есть, но у нас еще есть активные задачи на переноску материалов, то это ошибка            
        if (workpoint.materials.HasAllMaterials() == true && HasHaulingJobs()) {            
            Debug.LogError("Goal_WorkAtWorkstation: has hauling jobs, but all materials_at_place");
            //TOFIX: тут можно что то сделать чтобы перезапустить задачу
            return;            
        }

        //проверяем что у нас есть точка работы
        if(workpoint == null) {
            Debug.LogError("Goal_WorkAtWorkstation: workpoint is null");            
            return;
        }

        //TOFIX: очищаем станок от ненужных материалов

        //доставляем материалы для работы
        foreach(string mat in jobs_hauling.Keys) {

            JobInfo j = jobs_hauling[mat];

            //если нам все еще нужен материал, но задача на его доставку не выполняется и не в очереди  
            //то меняем статус задачи на ожидание (и таким образом отправляем ее в очередь задач в JobAgent)
            if (workpoint.materials.NeedMaterial(mat) > 0 
                && IsJobInStatus(j, JobStatus.Status.Running, JobStatus.Status.Wait) == false) {
                ChangeJobStatus(j, JobStatus.Status.Wait);
                continue;
            }            
        }

        //работаем
        if(workpoint.materials.HasAllMaterials() == true 
            && IsJobInStatus(job_work, JobStatus.Status.Running, JobStatus.Status.Wait) == false) {
            ChangeJobStatus(job_work, JobStatus.Status.Wait);
        }

        //TOFIX: доставляем полученный продукт на склад или бросаем рядом
    }    

    private bool HasHaulingJobs()
    {
        if(jobs == null) {
            Debug.LogError("Goal_WorkAtWorkstation.HasHaulingJobs: job list is null");
            return false;
        }

        foreach(JobInfo j in jobs) {
            //TOFIX: тип задачи должен храниться в JobInfo
            if((j is JobInfo_HaulItemToWorkpoint) && jobStatuses[j].currStatus != JobStatus.Status.Complete) {
                return true;
            }
        }

        return false;
    }

    //Помогает корректно сменить статус задачи.
    //Следит за размещением задачи в JobAgent
    private void ChangeJobStatus(JobInfo job, JobStatus.Status newStatus)
    {
        //Если задача только добавлена, то создаем для нее объект со статусом
        if(jobStatuses.ContainsKey(job) == false) {
            jobStatuses.Add(job, new JobStatus());
        }

        //Если статус не изменился, то ничего не делаем
        if(newStatus == jobStatuses[job].currStatus) {
            return;
        }

        //Если задача переходит в статус ожидания агента, то добавляем ее в очередь через JobAgent
        
        if(newStatus == JobStatus.Status.Wait) {
            JobAgent.GetInstance().AddJob(job);
        }

        //для всех остальных случаев, убираем задачу из списка задач
        if (newStatus != JobStatus.Status.Wait) {
            JobAgent.GetInstance().RemoveJob(job);
        }

        //меняем значение текущего статуса
        jobStatuses[job].currStatus = newStatus;
    }

    private bool IsJobInStatus(JobInfo job, params JobStatus.Status[] statuses)
    {
        foreach(JobStatus.Status s in statuses) {
            if (jobStatuses[job].currStatus == s) return true;
        }

        return false;
    }        

    public override string ToString()
    {
        StringBuilder sb = new StringBuilder();

        sb.Append("Goal(WorkAtWorpoint) ");

        sb.AppendFormat("HaulJobCount: {0}", jobs_hauling.Count);

        return sb.ToString();
    }
}
