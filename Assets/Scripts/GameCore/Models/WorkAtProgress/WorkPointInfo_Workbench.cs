using UnityEngine;
using System.Collections.Generic;

//Коллекция настроек для станков
public class WorkPointInfo_Workbench
{    
    public static WorkPointInfo Load(string objectType)
    {
        WorkbenchInfo workbenchInfo = RecipeManager.GetWorkbenchInfo(objectType);

        if(workbenchInfo == null) {
            Debug.LogErrorFormat("WorkPointInfo_Workbench: can't load info for {0}", objectType);
            return null;
        }

        //Очереди производства
        IWorkQueue queue = null;

        //Настраиваем очередь в зависимости от ее типа
        if (workbenchInfo.queue_id == "repeat") {
            
            //Повторяемая
            queue = new WorkQueue_Repeat(workbenchInfo.recipes);
        }        

        if(queue == null) {
            Debug.LogErrorFormat("WorkPointInfo_Workbench: can't load {0} workbench", objectType);
            return null;
        }

        WorkPointInfo info = queue.Next();        

        info.OnComplete = (workpoint) => { OnComplete(queue, workpoint); };
        info.OnStart = OnStart;
        info.OnChanged = OnChanged;
        info.OnCancel = OnCancel;        

        return info;
    }

    //Обработчик событие - начало производства
    //TOTHINK: это однократно при установке станка или в начале производства каждого предмета???
    private static void OnStart(WorkPoint workpoint)
    {
    }

    //Обработчик события - изменение прогресса производства
    private static void OnChanged(WorkPoint workpoint)
    {
        //TOFIX: анимация производства предмета
    }

    //Обработчик события - отмена производства
    private static void OnCancel(WorkPoint workpoint)
    {        
    }

    //Обработчик события - завершение производства    
    private static void OnComplete(IWorkQueue queue, WorkPoint workpoint)
    {
        List<KeyValuePair<string, int>> items = queue.Product();

        //Спавним полученные предметы
        for (int i = 0; i < items.Count; ++i) {
            ItemManagerA.SpawnItem(workpoint.spawnPoint, items[i].Key, items[i].Value);
        }

        //Переключемся на производство следующего предмета
        queue.Next();
    }
}
