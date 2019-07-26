using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class InstalledObjectSpriteController : MonoBehaviour
{
    //Соответствие InstalledObject к рисуемому объекту
    Dictionary<InstalledObject, GameObject> installedObjectGameObjectMap;

    //Функции для выбора и трансформации спрайта по указанному InstalledObject
    Dictionary<string, System.Func<InstalledObject, Sprite>> installedObjectSpriteFunc;
    Dictionary<string, System.Action<InstalledObject, SpriteRenderer>> InstalledObjectTransformFunc;

    //Список объектов нуждающихся в перерисовке
    List<InstalledObject> dirtyInstalledObjects;

    World world {
        get {
            return World.current;
        }
    }

	// Use this for initialization
	void Start ()
    {                
        installedObjectGameObjectMap = new Dictionary<InstalledObject, GameObject>();
        
        //Подписываемся на событие (создание объекта)
        world.cbInstalledObjectCreated += OnInstalledObjectCreated;

        //Подписываемся на событие перерисовки объектов
        world.cbObjectsRedraw += UpdateInstalledObjects;

        //Загружаем кастомные функции обновления спрайта
        LoadSpriteUpdateFunctions();

        // Go through any EXISTING furniture (i.e. from a save that was loaded OnEnable) and call the OnCreated event manually
        foreach (InstalledObject furn in world.installedObjects) {
            OnInstalledObjectCreated(furn);
        }

        //Создаем список под объекты для перерисовки
        dirtyInstalledObjects = new List<InstalledObject>(128);
    }    

    //Загружает список кастомных функций обновления спрайта
    void LoadSpriteUpdateFunctions()
    {
        installedObjectSpriteFunc = new Dictionary<string, System.Func<InstalledObject, Sprite>>();
        InstalledObjectTransformFunc = new Dictionary<string, System.Action<InstalledObject, SpriteRenderer>>();

        //функция получения спрайта для строительной прощадки
        installedObjectSpriteFunc.Add("BuildPlace", InstalledObjectSprite_BuildPoint.GetObjectSprite);
        InstalledObjectTransformFunc.Add("BuildPlace", InstalledObjectSprite_BuildPoint.UpdateSprite);

        //функция получения спрайта для горшка с кристалом
        //TOFIX: Функции обновления спрайта InstalledObjectSprite_Growable должны добавляться по наличию параметра
        //       grow_stage, grow_stage_max, grow_stage_speed ...
        installedObjectSpriteFunc.Add("CrystalPot", InstalledObjectSprite_Growable.GetObjectSprite);        

        //Функция рисования спрайта, если он рисуется в зависимости от соседей
        //TOFIX: Функции обновления спрайта InstalledObjectSprite_Linkable должны добавялться по наличию параметра
        //       linksToNeighbour
        installedObjectSpriteFunc.Add("Wall", InstalledObjectSprite_Linkable.GetObjectSprite);
    }

    void OnInstalledObjectCreated(InstalledObject obj)
    {        
        if(obj.objectType == null) {
            Debug.LogError("OnInstalledObjectCreated error: objectType is null");
            return;
        }

        if(obj.tile == null) {
            Debug.LogError("OnInstalledObjectCreated error: tile is null");
            return;
        }

        GameObject go = new GameObject();

        installedObjectGameObjectMap.Add(obj, go);

        go.name = string.Format("{0}_{1}_{2}", obj.objectType, obj.tile.x, obj.tile.y);
        go.transform.position = new Vector3(obj.tile.x + (obj.width - 1) / 2f, obj.tile.y + (obj.height - 1) / 2f, 0);
        go.transform.SetParent(this.transform, true);

        SpriteRenderer go_sr = go.AddComponent<SpriteRenderer>();        
        go_sr.sprite = GetInstalledObjectSprite(obj);
        go_sr.sortingLayerName = "InstalledObject";
        go_sr.color = obj.tint;

        //Меняем спрайт кастомной функцией, если она есть
        TransformSprite(obj, go_sr);

        //TOFIX: перенести настройку спрайта для двери(Door) в отдельный метод
        if (obj.objectType == "Door")
        {
            Tile north_tile = world[obj.tile.x, obj.tile.y + 1];
            Tile south_tile = world[obj.tile.x, obj.tile.y - 1];

            if (north_tile != null && south_tile != null && north_tile.installedObject != null && south_tile.installedObject != null
                && north_tile.installedObject.objectType == "Wall" && south_tile.installedObject.objectType == "Wall")
            {
                go.transform.localRotation = Quaternion.Euler(0, 0, 90f);                
            }            
        }

        obj.RegisterOnChanged(OnInstalledObjectChanged);
        obj.RegisterOnRemoved(OnInstalledObjectRemoved);
    }

    void OnInstalledObjectChanged(InstalledObject obj)
    {
        if(obj.isDirty == false) {
            obj.isDirty = true;
            dirtyInstalledObjects.Add(obj);
        }        
    }

    //Функция перерисовывает уже созданный объект
    void UpdateInstalledObject(InstalledObject obj)
    {
        //Correcting link graphics
        if (installedObjectGameObjectMap.ContainsKey(obj) == false) {
            Debug.LogError("OnInstalledObjectChanged cant find gameObject for installedObject");
            return;
        }

        installedObjectGameObjectMap[obj].GetComponent<SpriteRenderer>().sprite = GetInstalledObjectSprite(obj);

        //TOFIX: Трансформация спрайта после изменения состояния объекта

        if (obj.objectType == "Door") {
            installedObjectGameObjectMap[obj].transform.localScale = new Vector3(
                //1f - obj.installedObjectParameters[ParameterName.openness], 1f, 1f);
                1f - obj.parameters.GetFloat(ParameterName.openness), 1f, 1f);
        }
    }

    void UpdateInstalledObjects()
    {
        //Если нечего перерисовывать, сразу уходим
        if (dirtyInstalledObjects.Count == 0) return;

        //Перерисовываем объекты в списке
        for(int i=0;i<dirtyInstalledObjects.Count;++i) {
            InstalledObject obj = dirtyInstalledObjects[i];
            UpdateInstalledObject(obj);
            obj.isDirty = false;
        }

        //Очищаем список
        dirtyInstalledObjects.Clear();
    }

    void OnInstalledObjectRemoved(InstalledObject obj)
    {
        if(installedObjectGameObjectMap.ContainsKey(obj) == false) {
            Debug.LogError("OnInstalledObjectRemoved cant find gameObject for installedObject");
            return;
        }

        obj.UnRegisterOnChanged(OnInstalledObjectChanged);

        GameObject.Destroy(installedObjectGameObjectMap[obj]);        
        installedObjectGameObjectMap.Remove(obj);
    }

    private Sprite GetInstalledObjectSprite(InstalledObject obj)
    {
        if (obj == null) {
            return null;
        }
        
        //Если есть кастомная функция для обновления спрайта объекта, то спользуем ее        
        if (installedObjectSpriteFunc.ContainsKey(obj.objectType)) {
            return installedObjectSpriteFunc[obj.objectType](obj);
        }

        //иначе используем функцию по умолчанию        
        return SpriteCollection.GetInstalledObjectSprite(obj.objectType);
    }    

    //Получение спрайта для рисования превью конструкции
    public Sprite GetSpriteForInstalledObject(string objectType)
    {
        InstalledObject obj = World.current.installedObjectCollection.GetPrototype(objectType);

        Sprite sprite = GetInstalledObjectSprite(obj);

        if(sprite == null) {
            Debug.LogError("GetSpriteForInstalledObject -- no such sprite " + objectType);
        }

        return sprite;        
    }

    private void TransformSprite(InstalledObject obj, SpriteRenderer render)
    {
        //Debug.LogFormat("TransformSprite objType = {0}", obj.objectType);

        if(InstalledObjectTransformFunc.ContainsKey(obj.objectType)) {
            InstalledObjectTransformFunc[obj.objectType](obj, render);
        }
    }
}
