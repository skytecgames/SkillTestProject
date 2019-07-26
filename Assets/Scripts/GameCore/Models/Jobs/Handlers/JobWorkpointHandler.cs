using UnityEngine;
using UnityEditor;

//Класс связывает Job и WorkPoint (позволяет им обмениваться событиями)
public class JobWorkpointHandler
{
    private WorkPoint workpoint;
    private JobNew job;

    public JobWorkpointHandler(JobNew job, WorkPoint workpoint)
    {
        Init(job, workpoint);
    }

    //Инициализация (сделал с расчетом на будующий пул объектов)
    private void Init(JobNew job, WorkPoint workpoint)
    {
        this.job = job;
        this.workpoint = workpoint;

        workpoint.cbWorkCanceled += OnWorkPointCancel;
        job.cbJobComplete += OnJobComplete;
        job.cbJobStop += OnJobCancel;
    }

    //Открепить класс от задачи и точки работы
    private void Dispose()
    {
        if (workpoint != null) {
            workpoint.cbWorkCanceled -= OnWorkPointCancel;
            workpoint = null;
        }

        if(job != null) {
            job.cbJobComplete -= OnJobComplete;
            job.cbJobStop -= OnJobCancel;
            job = null;
        }
    }

    //Обработчик события завершения работы
    private void OnJobComplete(JobNew job)
    {
        Dispose();
    }

    //Обработчик события отмены работы
    private void OnJobCancel(JobNew job)
    {
        Dispose();
    }     

    private void OnWorkPointCancel(WorkPoint point)
    {
        if (job == null) {
            Debug.LogError("cancel workpoint");
            return;
        }

        job.Cancel();

        Dispose();        
    }
}