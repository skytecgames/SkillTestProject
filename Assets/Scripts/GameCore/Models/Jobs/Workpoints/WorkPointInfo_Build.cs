using UnityEngine;
using System.Collections.Generic;

//Коллекция настроек для работ по строительству предметов
public class WorkPointInfo_Build
{
    const string BuildPlaceObjectType = "BuildPlace";

    private static Dictionary<string, WorkPointInfo> collection = new Dictionary<string, WorkPointInfo>();

    //Возвращает настроки для строительства объекта указанного типа
    public static WorkPointInfo Load(string objectType)
    {
        Debug.LogFormat("WorkPointInfo_Build.Load: objectType = {0}", objectType);

        if (collection.ContainsKey(objectType)) return collection[objectType];

        //TOFIX: данные для описания материалов и работы для строительства должны браться из ресурсов игры

        WorkPointRecipe_Build recipe = RecipeManager.GetBuildRecipe(objectType);
        WorkPointInfo info = new WorkPointInfo();
        recipe.GetInfo(info);

        //Для строительства функции начала, изменения и отмены выглядят одинаково        
        info.OnStart = (workpoint) => { OnStart(recipe.objectType, workpoint); };
        info.OnChanged = OnChanged;
        info.OnCancel = OnCancel;

        //для завершения нужно передавать идентификатор объекта, который мы строим        
        info.OnComplete = (workpoint) => { OnComplete(recipe.objectType, workpoint); };

        collection.Add(objectType, info);

        return info;
    }

    //Обработчик событие - начала строительства
    private static void OnStart(string objectType, WorkPoint workpoint)
    {
        Debug.LogFormat("WorkPointInfo_Build: objectType = {0}", objectType);

        //Размещаем на месте строительства объект типа - строительная прощадка.

        //Получаем прототип объкта, который мы строим
        InstalledObject obj_proto = World.current.installedObjectCollection.GetPrototype(objectType);

        //Создаем прототип строительной площадки подходящих размеров
        InstalledObject buildplace = InstalledObject.CreatePrototype(BuildPlaceObjectType, 0f, 
            obj_proto.width, obj_proto.height);
        buildplace.jobSpotOffset = obj_proto.jobSpotOffset;
        buildplace.jobSpawnSpotOffset = obj_proto.jobSpawnSpotOffset;

        //Размещаем площадку
        InstalledObject obj = InstalledObject.PlaceInstance(buildplace, workpoint.tile);
        World.current.PlaceInstalledObject(obj, workpoint.tile);

        //Метод рисования для этого объекта берется из InstalledObjectSprite_BuildPoint
    }

    //Высчитываем насколько далеко продвинулся процесс строительства и обновляем вид строительной прощадки
    private static void OnChanged(WorkPoint workpoint)
    {        
        //Вычисляем прогресс строительства
        float progress = 0;
        InstalledObject obj = workpoint.tile.installedObject;
        if(workpoint.materials.HasAllMaterials() == false) {
            progress = 0;
        } else {
            //TODO: тут как то нужно получиться исходное значение работы
            int workMax = 10;
            progress = (workMax - workpoint.workAmount) / workMax;
        }

        //устанавливаем объекту соответствующий параметр
        obj.parameters.SetFloat(ParameterName.workProgress, progress);

        //оповещаем всех заинтерисованных, в том что спрайт изменился        
        obj.InvokeOnChanged();
    }

    private static void OnCancel(WorkPoint workpoint)
    {
        //TODO: отмена строительства (тут нужно понять к кому первому придет этот эвент и от него уже вызываться Cancel 
        //      у других классов)

        //TOTHINK: кажется что сюда мы должны прийти после вызова InstalledObject.Deconsruct
        //         значит мы должны подписаться на cbOnRemove              
    }

    //Обработчик события - завершение строительства
    private static void OnComplete(string ObjectType, WorkPoint workpoint)
    {
        if(workpoint.tile.installedObject == null) {
            Debug.LogError("Build point already removed");
            return;
        }

        if(workpoint.tile.installedObject.objectType != BuildPlaceObjectType) {
            Debug.LogError("BuildPoint site has wrong type or already removed");
            return;
        }

        //Удаляем строительную площадку
        workpoint.tile.installedObject.Deconstruct();

        //Размещаем постоенный объект
        World.current.PlaceInstalledObject(ObjectType, workpoint.tile);        
    }    
}
