using UnityEngine;
using System.Collections;

//класс связывает InstalledObject и WorkPoint так, чтобы по завершению работ вызвался метод завершения сбора ресурсов
public class GatherWorkPointHandler
{
    private InstalledObject obj;
    private WorkPoint point;

    public GatherWorkPointHandler(InstalledObject obj, WorkPoint point)
    {
        if(obj == null || point == null) {
            Debug.LogError("GatherWorkPointHandler init with null data");
            return;
        }

        this.obj = obj;
        this.point = point;        

        Init();
    }

    private void OnWorkPointComplete(WorkPoint workpoint)
    {
        if(point == null || obj == null) {
            Debug.LogError("GatherWorkPointHandler was cleared before complete");
            return;
        }        

        //Вызываем функцию завершения сбора ресурсов
        GatherTypeActions.Finish(obj.parameters);        

        Clear();
    }

    private void OnWorkPointCancel(WorkPoint workpoint)
    {
        Clear();
    }

    private void Init()
    {
        point.cbWorkComplete += OnWorkPointComplete;
        point.cbWorkCanceled += OnWorkPointCancel;
    }

    private void Clear()
    {
        if(point == null) {
            Debug.LogError("GatherWorkPointHandler already clean");
            return;
        }

        point.cbWorkComplete -= OnWorkPointComplete;
        point.cbWorkCanceled -= OnWorkPointCancel;

        obj = null;
        point = null;
    }
}
