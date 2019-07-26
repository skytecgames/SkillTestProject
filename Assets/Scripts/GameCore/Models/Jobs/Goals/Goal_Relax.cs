using UnityEngine;
using System.Collections;

//Задача - отдохнуть
public class Goal_Relax : Goal
{
    //задача на удовлетворение нужды персонажей в отдыхе
    //запускается в единственном экземпляре. Создает для агентов целевые задачи на отдых

    JobInfo wait_job;

    public override void Start()
    {
        //TOFIX: пока эта задача просто заставляет персонажа стоять на месте      
        //TOFIX: пока эта задача не отслеживает состояние персонажей и расчитана на одного персонажа

        wait_job = new JobInfo_Wait();
        wait_job.type = JobType.Wait;

        //тут нужен низкий приоритет, иначе наш персонаж все время будет делать эту задачу
        JobAgent.GetInstance().AddJob(wait_job);
    }    
}
