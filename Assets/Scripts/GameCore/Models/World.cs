using MiniJSON;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using UnityEngine;

public class World
{
    public static World current { get; protected set; }

    public int width { get; private set; }
    public int height { get; private set; }

    //все тайлы
    Tile[,] tiles;

    //все персонажи
    public List<Character> characters;

    //Все устанавливаемые объекты
    public List<InstalledObject> installedObjects;

    //Все комнаты
    public List<Room> rooms;

    //Список игровых процессов
    public List<IProcess> processes;

    //менеджер предметов
    public ItemManager itemManager;

    //Счетчик игрового времени
    public GameTime gameTime;    

    //Данные для поиска пути
    //public Path_TileGraph tileGraph;

    //Все текущие задачи    
    GoalManager goalManager;

    //прототипы устанавливаемых объектов    
    public InstalledObjectCollection installedObjectCollection;

    //События
    public System.Action<InstalledObject> cbInstalledObjectCreated;
    public System.Action<Tile> cbTileChanged;
    public System.Action<Character> cbCharacterCreated;    
    public System.Action<ItemContainer> cbItemContainerCreated;
    public System.Action cbObjectsRedraw;

    public World(int width, int height)
    {       
        SetupWorld(width, height);

        // Make test characters
        CreateCharacter(tiles[width / 2, height / 2]);
        CreateCharacter(tiles[width / 2 + 1, height / 2]);
    }

    void SetupWorld(int width, int height)
    {
        current = this;

        //Запускаем агента задач
        JobAgent jobAgent = new JobAgent();
        jobAgent.Init(this);

        //Запускаем менеджер целей
        goalManager = new GoalManager();
        goalManager.Init();

        //Инициализируем данные о рецептах
        RecipeManager.Init();

        //Инициализируем менеджер механики сбора ресурсов
        GatherManager.Init();

        //Инициализируем менеджер резервирования тайлов
        TileClaimManager.Init();

        //Инициализируем игровое поле
        this.width = width;
        this.height = height;

        tiles = new Tile[width, height];

        rooms = new List<Room>();
        rooms.Add(new Room());
        Room.OutsideRoom = rooms[0];

        for (int x = 0; x < width; ++x)
        {
            for (int y = 0; y < height; ++y)
            {
                tiles[x, y] = new Tile(x, y);
                tiles[x, y].RegisterOnChanged(OnTileChanged);
                tiles[x, y].room = Room.OutsideRoom;
            }
        }
                
        //Инициализируем коллекцию прототипов конструкций
        installedObjectCollection = new InstalledObjectCollection();        

        //Инициализируем списки объектов на игровом поле
        characters = new List<Character>();
        installedObjects = new List<InstalledObject>();
        itemManager = new ItemManager();       

        //Инициализируем игровое время
        gameTime = new GameTime();

        //Инициализируем необходимые для игры процессы
        processes = new List<IProcess>();

        //Инициализируем процесс управления ростом объектов
        GrowModel grow_process = new GrowModel();
        processes.Add(grow_process);

        //Инициализируем менеджер роста объектов
        GrowManager.Init(grow_process);

        //Инициализируем систему поиска пути
        PathfindingManager.Init(this);

        //Инициализируем менеджер манипуляций с предметами
        ItemManagerA.Init();
        ItemFinder.Init(this);

        Debug.LogFormat("World was created ({0},{1})", width, height);        
    }

    public void AddRoom(Room r)
    {
        rooms.Add(r);
    }

    public void DeleteRoom(Room r)
    {
        if (r != Room.OutsideRoom) {
            r.UnassignAllTiles();
            rooms.Remove(r);
        }
    }

    public void Update(float deltaTime)
    {
        //Обновляем время
        gameTime.Update(deltaTime);

        //Обновляем состояние всех персонажей
        foreach(Character c in characters) {
            c.Update(deltaTime);
        }

        //Обновляем состояние всех строений
        foreach(InstalledObject i in installedObjects) {
            i.Update(deltaTime);
        }
       
        //Обновляем все игровые процессы
        for(int i=0;i<processes.Count;++i) {
            processes[i].Update(deltaTime);
        }

        //Перерисовываем все что должно быть перерисовано
        if(cbObjectsRedraw != null) {
            cbObjectsRedraw();
        }
    }    

    public Character CreateCharacter(Tile tile)
    {
        Character c = new Character(tile);
        characters.Add(c);
        if (cbCharacterCreated != null) {
            cbCharacterCreated(c);
        }

        return c;
    }    

    //Размещение объекта по имени прототипа
    public InstalledObject PlaceInstalledObject(string objectType, Tile tile)
    {
        if (installedObjectCollection.HasPrototype(objectType) == false)
        {
            Debug.LogError("World error: такого типа устанавливаемых объектов не найдено в словаре ==> " + objectType);
            return null;
        }

        if(tile == null) {
            Debug.LogError("World.PlaceInstalledObject error: tile is null");
            return null;
        }

        InstalledObject obj = InstalledObject.PlaceInstance(installedObjectCollection.GetPrototype(objectType), tile);

        return PlaceInstalledObject(obj, tile);        
    }

    //Размещение объекта по прототипу
    public InstalledObject PlaceInstalledObject(InstalledObject obj, Tile tile)
    {
        if (obj == null) {
            return null;
        }              

        installedObjects.Add(obj);
        obj.RegisterOnRemoved(OnInstalledObjectRemoved);

        if (obj.parameters.GetFloat(ParameterName.room_enclosure) == 1f) {
            //Recalculate rooms
            Room.DoRoomFloodFill(obj.tile);
        }

        if (obj != null && cbInstalledObjectCreated != null) {
            cbInstalledObjectCreated(obj);
            //InvalidateTileGraph();            
        }

        return obj;
    }

    //Проверка возможности размещение объекта на указанном тайле
    public bool IsInstalledObjectPlacementValid(string objectType, Tile tile)
    {
        if (installedObjectCollection.HasPrototype(objectType) == false)
        {
            Debug.LogErrorFormat("World.IsInstalledObjectPlacementValid -- don't have prototype for {0}", objectType);
            return false;
        }

        return installedObjectCollection.GetPrototype(objectType).funcPositionValidation(tile);
    }

    public bool IsTileAtBounds(int x, int y)
    {
        if (tiles == null || x >= width || x < 0 || y >= height || y < 0)
        {
            return false;
        }

        return true;
    }

    public Tile this[int x, int y]
    {
        get {
            if(!IsTileAtBounds(x, y))
            {
                //Debug.LogErrorFormat("tiles {0}:{1} is out of bounds", x, y);
                return null;
            }

            return tiles[x,y];
        }
    }

    #region World generators

    public void GenRandom()
    {
        for (int x = 0; x < width; ++x)
        {
            for (int y = 0; y < height; ++y)
            {
                tiles[x, y].Type = (TileType)Random.Range(0, 2);
            }
        }
    }

    public void GenBlank()
    {
        for (int x = 0; x < width; ++x)
        {
            for (int y = 0; y < height; ++y)
            {
                tiles[x, y].Type = TileType.Empty;
            }
        }
    }

    public void GenPathTest()
    {
        GenBlank();

        int x1 = width / 2 - 10;
        int x2 = width / 2 + 10;

        int y1 = height / 2 - 10;
        int y2 = height / 2 + 10;

        for(int x = x1; x < x2; ++x) {
            for(int y = y1; y < y2; ++y)
            {
                tiles[x, y].Type = TileType.Floor;
            }
        }

        y1 = height / 2 - 5;
        y2 = height / 2 + 5;

        for (int x = x1; x < x2; ++x)
        {
            if (x == width / 2 + 5) continue;

            World.current.PlaceInstalledObject("Wall", tiles[x, y1]);
            World.current.PlaceInstalledObject("Wall", tiles[x, y2]);
        }

        y1 = height / 2 - 10;
        y2 = height / 2 + 10;
        x1 = width / 2 - 5;

        for (int y = y1; y < y2; ++y)
        {
            if (y == width / 2) continue;
            World.current.PlaceInstalledObject("Wall", tiles[x1, y]);
        }

    }

    #endregion

    void OnTileChanged(Tile t)
    {       
        if(cbTileChanged == null) {
            return;
        }
                
        cbTileChanged(t);       
        //InvalidateTileGraph();
    }

    void OnInstalledObjectRemoved(InstalledObject obj)
    {
        obj.UnRegisterOnRemoved(OnInstalledObjectRemoved);
        installedObjects.Remove(obj);
    }

    //public void InvalidateTileGraph()
    //{
    //    tileGraph = null;
    //}

    #region  ------ SAVE AND LOAD --------------------------------------------------------------------------------
    public World()
    {
    }

    public void Write(JSONObject json)
    {
        // Save info here
        json.AddInt("Width", width);
        json.AddInt("Height", height);

        //Записываем игровое время
        json.AddFloat("GameTime", GameTime.value);

        //Записываем тайлы
        JSONArray s_tiles = new JSONArray();        
        for (int x = 0; x < width; x++) {
            for (int y = 0; y < height; y++) {
                if (this[x, y].Type == TileType.Empty) continue;

                JSONObject s_tile = new JSONObject();
                
                tiles[x, y].Write(s_tile);

                s_tiles.Add(s_tile);
            }
        }
        json.AddArray("Tiles", s_tiles);
        
        //Записываем конструкты
        JSONArray s_installedObjects = new JSONArray();
        foreach (InstalledObject furn in installedObjects) {
            JSONObject s_installedObject = new JSONObject();
            furn.Write(s_installedObject);
            s_installedObjects.Add(s_installedObject);
        }        
        json.AddArray("InstalledObjects", s_installedObjects);
        
        //Записываем персонажей
        JSONArray s_characters = new JSONArray();
        foreach (Character c in characters) {
            JSONObject s_character = new JSONObject();            
            c.Write(s_character);
            s_characters.Add(s_character);
        }
        json.AddArray("Characters", s_characters);
    }

    public void Read(JSONObject json)
    {
        width = json.GetInt("Width");

        height = json.GetInt("Height");

        GameTime.value = (float)json.GetNumber("GameTime");
        
        SetupWorld(width, height);

        JSONArray s_tiles = json.GetArray("Tiles");
        Read_Tiles(s_tiles);

        JSONArray s_installedObjects = json.GetArray("InstalledObjects");
        Read_InstalledObjects(s_installedObjects);

        JSONArray s_characters = json.GetArray("Characters");
        Read_Characters(s_characters);
    }

    private void Read_Tiles(JSONArray json)
    {
        if (json == null) return;

        foreach(object o in json) {
            JSONObject t = new JSONObject(o);
            int x = t.GetInt("X");
            int y = t.GetInt("Y");
            tiles[x, y].Read(t);
        }        
    }

    private void Read_InstalledObjects(JSONArray json)
    {
        if (json == null) return;

        foreach(object o in json) {
            JSONObject io = new JSONObject(o);

            int x = io.GetInt("X");
            int y = io.GetInt("Y");

            //TOTHINK: при загрузке конструктов сначала вызывается onCreate с базовым прототипом, а потом onChanged
            //         это не оптимально (дважды происходит обработка в процессах), нужно сначала загрузить все
            //         параметры, а потом вызывать создание по экземпляру
            //         проблема в том что у нас нет метода размещения по экземпляру

            InstalledObject furn = PlaceInstalledObject(io.GetString("objectType"), tiles[x, y]);
            furn.Read(io);
            furn.parameters.InvokeOnChanged();
        }
    }

    private void Read_Characters(JSONArray json)
    {
        if (json == null) return;

        foreach(object o in json) {
            JSONObject cj = new JSONObject(o);
            int x = cj.GetInt("X");
            int y = cj.GetInt("Y");

            Character c = CreateCharacter(tiles[x, y]);
            c.Read(cj);
        }        
    }

    #endregion 
}
