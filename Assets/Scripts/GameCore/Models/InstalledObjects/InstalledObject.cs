using System;
using System.Xml;
using UnityEngine;
using System.Collections.Generic;

public partial class InstalledObject
{
    public Tile tile { get; private set; }

    //идентификатор объекта (спрайт)
    public string objectType { get; protected set; }

    //Множитель движения через данный объект, если > 20 то тайл не проходимый (стена)
    //Множители движения от всех препятствий на тайле складываются
    public float movementCost { get; protected set; }    

    //размеры объекта
    public int width { get; protected set; }
    public int height { get; protected set; }

    //цвет объекта
    public Color tint = Color.white;    

    //событие - объект создан
    public Action<InstalledObject> cbOnCreate;    
    //проверка на возможность уставновить объект на тайл
    public Func<Tile, bool> funcPositionValidation;

    //Кастомные параметры объекта и функции для их обработки    
    public IParameters parameters;
    public Action<InstalledObject, float> updateActions;    

    //Функция проверки и запроса доступа в тайл на котором установлен объект
    public Func<InstalledObject, Enterability> IsEnterable;    

    //смещение по которому расположено место, которое должен использовать персонаж для работы на этом объекте
    public Vector2 jobSpotOffset = Vector2.zero;
    //смещение по которому будут размещатся производимые предметы
    public Vector2 jobSpawnSpotOffset = Vector2.zero;

    //Флаг, показывающий что этот объект необходимо перерисовать
    //TOTHINK: флаг isDirty относится к работе контроллеров, но я не знаю как от него здесь уйти
    public bool isDirty = false;

    protected InstalledObject()
    {        
        parameters = new Parameters();
        updateActions = null;        
    }

    protected InstalledObject(InstalledObject proto)
    {
        this.tint = proto.tint;
        this.objectType = proto.objectType;
        this.movementCost = proto.movementCost;
        this.width = proto.width;
        this.height = proto.height;
        this.jobSpotOffset = proto.jobSpotOffset;
        this.jobSpawnSpotOffset = proto.jobSpawnSpotOffset;
                
        //копируем параметры и связываем события на изменение
        this.parameters = proto.parameters.Clone();
        this.parameters.RegisterOnChanged(this.OnParametersChanged);

        //Клонируем функцию обновления
        if (proto.updateActions != null) {
            this.updateActions = (Action<InstalledObject, float>)proto.updateActions.Clone();
        }

        //Мы не клонируем событие создания, так как мы вызываем его только раз и можем это сделать с прототипа        

        if (proto.funcPositionValidation != null)
        {
            this.funcPositionValidation = (Func<Tile, bool>)proto.funcPositionValidation.Clone() ;
        }

        this.IsEnterable = proto.IsEnterable;        
    }

    public static InstalledObject CreatePrototype(string objectType, float movementCost, int width, int height)
    {
        InstalledObject obj = new InstalledObject();

        obj.objectType = objectType;
        obj.movementCost = movementCost;
        obj.width = width;
        obj.height = height;

        obj.funcPositionValidation = obj.IsValidPosition;

        return obj;
    }

    public static InstalledObject PlaceInstance(InstalledObject proto, Tile tile)
    {
        if(proto.funcPositionValidation(tile) == false) {
            Debug.LogError("Place installed object on wrong tile");
            return null;
        }

        InstalledObject obj = new InstalledObject(proto);

        obj.tile = tile;

        if(tile.PlaceObject(obj) == false) {
            //can't place object, already ocupied?
            return null;
        }        

        //Отображаем с учетом соседей, если нужно
        obj.LinkToNeighbour();

        //Уведомляем всех о том, что мы установили объект на тайл
        if(proto.cbOnCreate != null) {
            Debug.LogFormat("IstalledObject.OnCreate type = {0}", obj.objectType);
            proto.cbOnCreate(obj);
        }

        return obj;
    }

    //проверка тайла на возможность установить на него объект
    bool IsValidPosition(Tile t)
    {
        for (int x_off = t.x; x_off < t.x + width; ++x_off)
            for (int y_off = t.y; y_off < t.y + height; ++y_off) {
                Tile t2 = World.current[x_off, y_off];

                if (t2.Type != TileType.Floor)
                {
                    return false;
                }

                if (t2.installedObject != null)
                {
                    return false;
                }
            }        

        return true;
    }    

    public bool IsStockpile()
    {
        return objectType == "Stockpile";
    }

    //функция обновления состояния объекта
    public void Update(float deltaTime)
    {
        if (updateActions != null) {
            updateActions(this, deltaTime);
        }
    }              

    public Tile GetJobSpotTile()
    {
        return World.current[tile.x + (int)jobSpotOffset.x, tile.y + (int)jobSpotOffset.y];
    }

    public Tile GetSpawnSpotTile()
    {
        return World.current[tile.x + (int)jobSpawnSpotOffset.x, tile.y + (int)jobSpawnSpotOffset.y];
    }    

    //функция для разрушения объекта
    public void Deconstruct()
    {
        Debug.LogFormat("Deconstruct installedObject {0}", objectType);
                
        //Открепляем объект от тайла (tile.installedObject) с учетом размера объекта
        tile.RemoveObject();

        //Если мы удалил стену комнаты, пересчитываем комнату
        //TOTHINK: вообще можно переложить эту функциональность на отдельный объект - менеджер комнат
        //         достаточно будет его подписать на событие OnRemove
        if(parameters.GetFloat(ParameterName.room_enclosure) == 1f) {
            Room.DoRoomFloodFill(tile);
        }

        //Помечаем граф поиска пути как недействительный
        //World.current.InvalidateTileGraph();        

        //отпишемся от событий обновляения        
        updateActions = null;        

        //Уведомляем об удалении объекта
        if(cbOnRemove != null) {
            cbOnRemove(this);
        }
    }

    //Если параметры объекта поменялись, то уведомляем об изменении объекта
    private void OnParametersChanged(IParameters obj)
    {        
        InvokeOnChanged();
    }

    // ------ SAVE AND LOAD

    public void Write(MiniJSON.JSONObject json)
    {
        json.AddInt("X", tile.x);
        json.AddInt("Y", tile.y);
        json.AddString("objectType", objectType);

        //TOFIX: избавиться от зависимости от негенерируемых констант при сериализации
        MiniJSON.JSONArray s_params = new MiniJSON.JSONArray();        
        for(int i=1; i<ParameterName.maxKey;++i) {
            float par = parameters.GetFloat(i);
            if(par != 0) {
                MiniJSON.JSONObject s_par = new MiniJSON.JSONObject();
                s_par.AddInt("key", i);
                s_par.AddFloat("value", par);
                s_params.Add(s_par);
            }
        }

        json.AddArray("FloatParams", s_params);
    }

    public void Read(MiniJSON.JSONObject json)
    {
        MiniJSON.JSONArray j_params = json.GetArray("FloatParams");
        if (j_params != null) {
            foreach (object o in j_params) {
                MiniJSON.JSONObject j_par = new MiniJSON.JSONObject(o);

                int key = j_par.GetInt("key");
                float val = (float)j_par.GetNumber("value");

                parameters.SetFloat(key, val);
            }
        }        
    }
}