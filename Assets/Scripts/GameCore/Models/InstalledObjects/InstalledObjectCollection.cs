using UnityEngine;
using System.Collections.Generic;

public class InstalledObjectCollection
{
    //прототипы устанавливаемых объектов
    public Dictionary<string, InstalledObject> installedObjectPrototypes;
    
    //Конструктор коллекции
    public InstalledObjectCollection()
    {
        Init();
    }

    //Функция загрузки прототипов устанавливаемых объектов
    private void Init()
    {
        // везде где мы используем подписку на cbOnCreate мы не отписываемся от этого события
        // cbOnCreate вызывается с прототипа

        installedObjectPrototypes = new Dictionary<string, InstalledObject>();
        string name;
        InstalledObject_Workbench workbench;

        //Прототип стены
        name = "Wall";
        installedObjectPrototypes.Add(name, InstalledObject.CreatePrototype(name, Tile.walkabilityMax, 1, 1));
        installedObjectPrototypes[name].parameters.SetFloat(ParameterName.links_to_neighbour, 1f);
        installedObjectPrototypes[name].parameters.SetFloat(ParameterName.room_enclosure, 1f);

        //Прототип двери
        name = "Door";
        installedObjectPrototypes.Add(name, InstalledObject.CreatePrototype(name, 0f, 1, 1));
        installedObjectPrototypes[name].parameters.SetFloat(ParameterName.openness, 0f);
        installedObjectPrototypes[name].parameters.SetFloat(ParameterName.is_opening, 0f);
        installedObjectPrototypes[name].parameters.SetFloat(ParameterName.room_enclosure, 1f);
        //TOFIX: [OPTIMISE] Для объектов с параметром openness не обязательно постоянно использовать updateAction
        //       достаточно делать обновление только при openness > 0. Добавлять метод можно при вызове IsEnterable
        //       и удалять при полном закрытии
        installedObjectPrototypes[name].updateActions += InstalledObjectActions.Door_UpdateAction;
        installedObjectPrototypes[name].IsEnterable = InstalledObjectActions.Door_IsEnterable;

        //прототип хранилища
        name = "Stockpile";
        installedObjectPrototypes.Add(name, InstalledObject.CreatePrototype(name, 0f, 1, 1));
        installedObjectPrototypes[name].cbOnCreate += InstalledObject_Stockpile.Stockpile_OnCreate;
        installedObjectPrototypes[name].tint = Color.red;

        //Прототип генератора кислорода
        name = "O2Generator";
        installedObjectPrototypes.Add(name, InstalledObject.CreatePrototype(name, 10f, 2, 2));
        //TOFIX: механики подобные наполнению комнаты кислородом не должны быть свойством конструкта
        //       они должны быть свойством комнаты. Представим что у нас был генератор кислорода в комнате и его удалили.
        //       Получится что кислород в комнате зафиксируется, а он должен начать уменьшаться
        installedObjectPrototypes[name].updateActions += InstalledObjectActions.OxygenGenerator_UpdateAction;

        //прототип консоли управления добычей руды
        name = "MiningStation";
        installedObjectPrototypes.Add(name, InstalledObject.CreatePrototype(name, 0f, 3, 3));        
        workbench = new InstalledObject_Workbench();        
        workbench.work_info = WorkPointInfo_Workbench.Load(name);
        installedObjectPrototypes[name].cbOnCreate += workbench.Workbench_OnCreate;
        installedObjectPrototypes[name].jobSpotOffset = new Vector2(1, 0);
        installedObjectPrototypes[name].jobSpawnSpotOffset = new Vector2(2, 0);

        //Прототип станции переработки руды в метал
        name = "SmithBench";
        installedObjectPrototypes.Add(name, InstalledObject.CreatePrototype(name, 0f, 3, 2));
        workbench = new InstalledObject_Workbench();        
        workbench.work_info = WorkPointInfo_Workbench.Load(name);
        installedObjectPrototypes[name].cbOnCreate += workbench.Workbench_OnCreate;
        installedObjectPrototypes[name].jobSpotOffset = new Vector2(1, 0);
        installedObjectPrototypes[name].jobSpawnSpotOffset = new Vector2(2, 0);

        //Прототип емкости с растущим кристалом
        name = "CrystalPot";
        installedObjectPrototypes.Add(name, InstalledObject.CreatePrototype(name, 0f, 1, 1));
        installedObjectPrototypes[name].cbOnCreate += InstalledObject_Grow.OnCreate;
        installedObjectPrototypes[name].parameters.SetFloat(ParameterName.grow_stage, 1);       //начальная стадия роста
        installedObjectPrototypes[name].parameters.SetFloat(ParameterName.grow_stage_max, 3);   //количество стадий
        installedObjectPrototypes[name].parameters.SetFloat(ParameterName.grow_speed, 5);       //сек на смену стадии
        installedObjectPrototypes[name].parameters.SetFloat(ParameterName.grow_stage_time, -1); //время след стадии
        installedObjectPrototypes[name].parameters.SetFloat(ParameterName.grow_is_active, 0);   //не растет в данный момент
        installedObjectPrototypes[name].cbOnCreate += InstalledObject_Gather.OnCreate;
        installedObjectPrototypes[name].parameters.SetFloat(ParameterName.gather_type, GatherTypeActions.GatherType_FullyGrown);
        installedObjectPrototypes[name].parameters.SetFloat(ParameterName.gather_finish_type, GatherTypeActions.GatherFinishType_ResetStage);
    }

    //Получение прототипа по идентификатору
    public InstalledObject GetPrototype(string objectType)
    {
        if (installedObjectPrototypes.ContainsKey(objectType) == false) {
            Debug.LogError("no installed object prototype = " + objectType);
            return null;
        }

        return installedObjectPrototypes[objectType];
    }

    //Проверка наличия прототипа с указанным идентификатором
    public bool HasPrototype(string objectType)
    {
        return installedObjectPrototypes.ContainsKey(objectType);
    }

    //Получение итератора коллекции
    //TOFIX: когда меню со списком объектов для строительства станет контекстным этот метод должен исчезнуть
    public Dictionary<string, InstalledObject>.KeyCollection GetIterator()
    {
        return installedObjectPrototypes.Keys;
    }
}
