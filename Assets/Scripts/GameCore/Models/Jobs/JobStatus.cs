using UnityEngine;
using System.Collections;

public class JobStatus
{
    //Список статусов для задач
    public enum Status {
        None,       //Задание не проинициализировано
        Wait,       //Задание ожидает агента в очереди
        Running,    //На задание назначен агент и оно выполняется в данный момент
        Complete    //Задание завершено и не в очереди
    }

    //Текущий статус задачи    
    public Status currStatus = Status.None;
}
