using UnityEngine;
using System.Collections.Generic;

//В этот класс размещаются все задачи, которые нужно выполнить в данный момент
//Сюда приходят Agent чтобы получить для себя задание
//Сюда Goal добавляют новые задания, которые требуют выполнения
//Здесь расчитывается приоритет заданий
public class JobAgent
{
    #region GetInstance

    private static JobAgent instance = null;
    public static JobAgent GetInstance()
    {
        return instance;
    }

    #endregion

    //Список задач ожидающих агента
    List<JobInfo> pending_jobs;

    //Инициализация класса
    public void Init(World world)
    {
        instance = this;

        world.cbCharacterCreated += OnCharacterCreated;

        //TOFIX: событие на удаление персонажа
        //world.cbCharacterDeleted += OnCharacterDeleted;

        pending_jobs = new List<JobInfo>();

        //Инициализируем таблицу приоритетов задач
        InitPriorityTable();
    }

    //Поиск новой задачи для персонажа (Agent)
    public void FindJob(Agent agent)
    {        
        JobInfo currJob = null;
        int currPriority = int.MaxValue;

        //Перебираем все задачи и ищем среди них наиболее приоритетную для данного персонажа
        for(int i = 0; i < pending_jobs.Count; ++i)
        {
            JobInfo job = pending_jobs[i];
            int priority = CalculatePriority(agent, job);

            if(priority < currPriority) {

                //TOFIX: проверка условий выполнения - трудоемкая функция.
                //       Чтобы избежать ее частого вызова мы сначала должны найти задачу с максимальным приоритетом,
                //       а потом проверять условия, если задача не подошла берем следующую и так далее
                if (job.CheckRequirements(agent)) {
                    currJob = job;
                    currPriority = priority;
                }
            }
        }

        //если мы не нашли никакой задачи для текущего пользователя, то это косяк
        if(currJob == null) {
            Debug.LogError("job manager can't find new job for character");
            return;
        }

        //назначаем агенту выбранную работу
        JobNew job_instance = currJob.PlanJob(agent);        
    }

    //Добавление новой задачи в список на выполнение (Goal)
    public void AddJob(JobInfo job)
    {        
        //Если задача уже в списке, не добавляем ее
        if(pending_jobs.Contains(job)) {
            Debug.LogError("job already in pending list");
            return;
        }        

        pending_jobs.Add(job);

        Debug.LogFormat("JobAgent.AddJob {0}, current pending jobs count {1}", job.type, pending_jobs.Count);
    }

    //Удаление задачи из списка на выполнение (Goal)
    public void RemoveJob(JobInfo job)
    {        
        //если задачи нет в списке ругаемся
        if(pending_jobs.Contains(job) == false) {
            Debug.LogWarning("job not in list (can't remove)");
            return;
        }

        pending_jobs.Remove(job);
    }

    //Обработка события на создание нового персонажа
    private void OnCharacterCreated(Character character)
    {
        //TOFIX: Реализовать реакцию на добавление персонажа в игру (учет всех персонажей в JobAgent)

        //Инициализируем агента для персонажа
        character.agent = new Agent();
        character.agent.character = character;        
    }

    //обработка событие на удаление персонажа
    private void OnCharacterDeleted(Character character)
    {
        //TOFIX: реализовать реакцию на событие на удаление персонажа, когда механика удаления будет реализована (JobAgent)
    }

    Dictionary<JobType, int> priorityTable = null;

    private void InitPriorityTable()
    {
        //TOFIX: приоритеты должны настраиваться пользователем

        priorityTable = new Dictionary<JobType, int>();

        //Строительство
        priorityTable.Add(JobType.Construction, 14);        

        //Производство
        priorityTable.Add(JobType.Crafting, 16);

        //Сбор ресурсов
        priorityTable.Add(JobType.Gather, 16);

        //Переноска предметов
        priorityTable.Add(JobType.Haul, 14);

        //Ожидание всегда имеет наименьший приоритет
        priorityTable.Add(JobType.Wait, 20);
    }

    //Функция вычисления приоритета задачи для указанного персонажа
    //Чем меньше возвращаемое число, тем более приоритетная задача
    private int CalculatePriority(Agent agent, JobInfo job)
    {        
        if(priorityTable.ContainsKey(job.type)) {
            return priorityTable[job.type];
        }

        return 0;
    }
}
