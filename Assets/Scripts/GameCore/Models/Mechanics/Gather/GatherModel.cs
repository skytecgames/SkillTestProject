using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Класс для управления механикой сбора ресурсов
public class GatherModel : IGatherFinder, IObjectRegister
{
    //конструкты готовые к сбору
    private List<InstalledObject> installedObjects;

    //конструкты для которых была создана точка работы
    private Dictionary<InstalledObject, WorkPoint> installedOnjectToWorkpointMap;

    public GatherModel()
    {
        installedObjects = new List<InstalledObject>();
        installedOnjectToWorkpointMap = new Dictionary<InstalledObject, WorkPoint>();
    }

    //Поиск точки сбора
    public WorkPoint GetClosestGatherPoint(Tile start)
    {
        //TOFIX: Поиск объекта для Gather механики долден быть переделан после переработки системы поиска пути

        //1. Ищем ближайший объект среди зарегистрированных
        InstalledObject obj_min = null;
        int dist_min = int.MaxValue;

        InstalledObject obj;
        int dist;

        for (int i = 0; i < installedObjects.Count; ++i) {

            obj = installedObjects[i];
            dist = Mathf.Abs(obj.tile.x - start.x) + Mathf.Abs(obj.tile.y - start.y);

            //TOFIX: тут нужно проверить что текущий объект не используется в данный момент
            //       мы не можем опереться на наличие workpoint, ведь он выдается по факту проведения поиска, 
            //       а не по факту использования в задаче

            //Проверяем, не занят ли этот объект другой задачей на сбор
            if(TileClaimManager.IsFree(obj.tile, ClaimType.Gather) == false) {
                continue;
            }

            if (obj_min == null || dist_min > dist) {
                obj_min = obj;
                dist_min = dist;
            }
        }

        //Если нет подходящих объектов, то возвращаем null
        if (obj_min == null) return null;

        //2. Ищем точку работы
        if(installedOnjectToWorkpointMap.ContainsKey(obj_min)) {
            return installedOnjectToWorkpointMap[obj_min];
        }

        //3.если нет точки работы, то ее нужно создать и добавить в коллекцию
        WorkPointInfo work_info = WorkPointInfo_Gather.Load(obj_min.objectType);
        WorkPoint workpoint = new WorkPoint();
        workpoint.spawnPoint = workpoint.tile = obj_min.tile;
        work_info.Init(workpoint);

        //Создадим объект, который проследит за вызовом функции завершения сбора ресурса
        GatherWorkPointHandler handler = new GatherWorkPointHandler(obj_min, workpoint);

        installedOnjectToWorkpointMap.Add(obj_min, workpoint);

        return workpoint;
    }

    public void RegisterObject(InstalledObject obj)
    {
        if(installedObjects == null) {
            Debug.LogError("Error: register object in gather model before init");
            return;
        }

        if (installedObjects.Contains(obj)) {
            Debug.LogError("Warning: object already registered");
            return;
        }

        Debug.LogFormat("GatherModel.RegisterObject");

        //Устанавливает обработчики событий для объекта
        obj.RegisterOnChanged(OnInstalledObjectChanged);
        obj.RegisterOnRemoved(OnInstalledObjectRemoved);

        //Вызывает функцию изменения для проверки, нельзя ли уже собрать объект
        OnInstalledObjectChanged(obj);
    }

    //Событие: конструкт с механикой сбора изменился
    private void OnInstalledObjectChanged(InstalledObject obj)
    {
        //Debug.LogFormat("GatherModel.OnInstalledObjectChanged time = {0}", GameTime.value);        

        //Проверяем готов ли объект к сбору и его наличие в списке объектов готовых к сбору
        if(GatherTypeActions.IsReady(obj.parameters) && installedObjects.Contains(obj) == false) {
            Debug.LogFormat("object {0} is ready to gather", obj.objectType);
            installedObjects.Add(obj);
        } else if(GatherTypeActions.IsReady(obj.parameters) == false && installedObjects.Contains(obj)) {
            Debug.LogFormat("object {0} is not gatherable", obj.objectType);

            //нужно очистить workpoint связанный с объектом, если такой есть (вызвать Cancel)
            ClearWorkPoint(obj);

            installedObjects.Remove(obj);
        }        
    }

    //Событие: конструкт с механикой сбора удален
    private void OnInstalledObjectRemoved(InstalledObject obj)
    {
        //Отпысываемся от событий
        obj.UnRegisterOnChanged(OnInstalledObjectChanged);
        obj.UnRegisterOnRemoved(OnInstalledObjectRemoved);

        //удаляем из списка готовых к сбору, если нужно
        if(installedObjects.Contains(obj)) {
            installedObjects.Remove(obj);
        }

        //очищаем связанный workpoint (вызываем Cancel)
        ClearWorkPoint(obj);
    }

    //Очистка WorkPoint (отвязка от конструкта)
    private void ClearWorkPoint(InstalledObject obj)
    {
        //проверяем наличие связанной с объектом точки работы
        if(installedOnjectToWorkpointMap.ContainsKey(obj)) {
            
            //отменяем точку работы
            installedOnjectToWorkpointMap[obj].Cancel();

            //удаляем привязку
            installedOnjectToWorkpointMap.Remove(obj);
        }
    }
}
