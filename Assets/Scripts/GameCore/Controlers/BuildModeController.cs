using UnityEngine;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public enum BuildMode
{
    TILE,
    OBJECT,
    DECONSTRUCT
}

public class BuildModeController : MonoBehaviour
{        
    public BuildMode buildMode = BuildMode.TILE;
    TileType buildModeTile = TileType.Floor;
    public string buildModeObjectType = string.Empty;    
        
    //ссылка на контроллер мышью
    MouseController mouseController;
    

    private void Start()
    {        
        //создаем ссылки на другие необходимые контролеры
        mouseController = FindObjectOfType<MouseController>();        
    }    

    public void SetMode_BuildFloor()
    {
        buildMode = BuildMode.TILE;
        buildModeTile = TileType.Floor;

        mouseController.objectTypeNotDragable = IsObjectDragable() == false;
        mouseController.StartBuildMode();
    }

    public void SetMode_Buldoze()
    {
        buildMode = BuildMode.DECONSTRUCT;
        buildModeTile = TileType.Empty;

        mouseController.objectTypeNotDragable = IsObjectDragable() == false;
        mouseController.StartBuildMode();
    }

    public void SetMode_BuildInstalledObject(string objectType)
    {
        buildMode = BuildMode.OBJECT;
        buildModeObjectType = objectType;

        mouseController.objectTypeNotDragable = IsObjectDragable() == false;
        mouseController.StartBuildMode();
    }

    public void SetMode_DeconstructObject()
    {        
        buildMode = BuildMode.DECONSTRUCT;

        mouseController.objectTypeNotDragable = IsObjectDragable() == false;
        mouseController.StartBuildMode();
    }

    public void PathfindingTest()
    {
        World.current.GenPathTest();        
    }

    public void DoBuild(Tile t)
    {
        Debug.LogFormat("DoBuild [{0}:{1}] Mode={2}", t.x, t.y, buildMode);

        //Команда на строительство объекта
        if (buildMode == BuildMode.OBJECT)
        {
            //Проверяем возможность разместить объект
            if (World.current.IsInstalledObjectPlacementValid(buildModeObjectType, t) == false)                
            {                
                return;
            }

            //Получаем тип объекта для размещения
            string installedObjectType = buildModeObjectType;            

            //создаем новую задачу
            Goal_WorkAtWorkstation goal = new Goal_WorkAtWorkstation(JobType.Construction);

            //настойка Goal под задачи строительства
            WorkPointInfo work_info = WorkPointInfo_Build.Load(installedObjectType);
            WorkPoint workpoint = new WorkPoint();
            workpoint.tile = t;
            work_info.Init(workpoint);            

            goal.workpoint = workpoint;
            goal.workpoint_info = work_info;

            //Запуск задачи
            goal.Start();

            Debug.Log(goal.ToString());
        }
        else if(buildMode == BuildMode.TILE) {
            t.Type = buildModeTile;
        } else if(buildMode == BuildMode.DECONSTRUCT) {            
            if(t.installedObject != null) {
                t.installedObject.Deconstruct();
            }
        } else {
            Debug.LogError("unimlemented buildmode");
        }
    }    

    bool IsObjectDragable()
    {
        if(buildMode == BuildMode.TILE || buildMode == BuildMode.DECONSTRUCT) {
            return true;
        }

        //TOFIX: перенести признак Dragable в прототип объекта
        InstalledObject proto = World.current.installedObjectCollection.GetPrototype(buildModeObjectType);
        if(proto.width > 1 || proto.height > 1) {
            return false;
        }

        return true;
    }
}    
