using UnityEngine;
using System.Collections;

public class Agent
{
    //данные персонажа
    public Character character;

    //текущее задание
    public JobNew currJob;

    //Установка новой задачи
    public void SetJob(JobNew job)
    {
        //Debug.LogFormat("character {0} get new job {1}", "test char", job.GetType().Name);

        //Если у нас есть предыдущая задача, отказываемся от нее
        ClearJob();

        //Подписываемся на новую работу
        currJob = job;
        currJob.cbJobComplete += OnJobComplete;
        currJob.cbJobStop += OnJobStop;

        currJob.Init();
    }

    //Очистка данных о текущей задачи (отписка от событий)
    private void ClearJob()
    {
        if (currJob != null) {
            currJob.cbJobComplete -= OnJobComplete;
            currJob.cbJobStop -= OnJobStop;
        }

        currJob = null;
    }

    //функция обновления задач
    public void Update(float deltaTime)
    {        
        //Если не выполняем в данный момент никаких задач, то берем себеновую задачу
        if(currJob == null) {
            JobAgent.GetInstance().FindJob(this);
        }

        currJob.Update(deltaTime);
    }    

    //Обработчик события - задача успешно завершена
    private void OnJobComplete(JobNew job)
    {
        //Debug.LogFormat("Agent.OnJobComplete ({0})", job.type);

        ClearJob();
    }

    //Обработчик события - выполнение задачи преостановлено
    private void OnJobStop(JobNew job)
    {
        ClearJob();
    }
}
