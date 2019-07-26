using UnityEngine;
using Pathfinding;

public enum TileType {
    Empty = 0,
    Floor = 1
}

public enum Enterability
{
    Yes, Never, Soon
}

public partial class Tile : IMoveCost
{
    //параметр проходимости после которого тайл считается непреодолимым
    public const float walkabilityMax = 20f;    

    //координаты тайла
    public int x { get; protected set; }
    public int y { get; protected set; }

    public float movementCost
    {
        get
        {
            if (type == TileType.Empty) return Tile.walkabilityMax;

            if(installedObject == null) {
                return 1f;
            }

            return installedObject.movementCost + 1f;            
        }
    }

    //Функция проверки и запроса разрешения на вход в тайл (для дверей например)
    public Enterability isEnterable
    {
        get
        {
            if (movementCost >= Tile.walkabilityMax) return Enterability.Never;

            //проверим не блокирует ли проход эффект от установленного объекта (временный)
            if(installedObject != null && installedObject.IsEnterable != null) {                               
                return installedObject.IsEnterable(installedObject);
            }

            return Enterability.Yes;
        }
    }

    //установленный на тайл объект
    public InstalledObject installedObject { get; protected set; }
    //комната в которой находится тайл
    public Room room;

    //TOFIX: Вместо прямой ссылки на контейнер тут нужен класс прокси.
    //       Нельзя создать контейнеры для всех тайлов, так как они содержат списки и могу занимать много места
    //       Но при этом хотелось бы иметь возможность обращаться к любому тайлу так, как будто на нем есть контейнер

    //Предметы в тайле (на полу)    
    public ItemContainer itemContainer;        

    private TileType type = TileType.Empty;       

    public TileType Type
    {
        get { return type; }
        set
        {
            //TOFIX: Если тип тайла совпадает с новым, ничего не делаем
            //if (type == value) return;

            type = value;
                        
            InvokeOnChanged();
        }
    }

    public Tile(int x, int y)
    {        
        this.x = x;
        this.y = y; 
    }

    public bool PlaceObject(InstalledObject objInstance)
    {
        //удаление объекта
        if(objInstance == null) {
            return RemoveObject();
        }

        //проверим можно ли установить тут объект
        if(objInstance.funcPositionValidation(this) == false) {
            return false;
        }

        //устанавливаем объект
        for (int x_off = x; x_off < x + objInstance.width; ++x_off)
            for (int y_off = y; y_off < y + objInstance.height; ++y_off)
            {
                Tile t = World.current[x_off, y_off];
                t.installedObject = objInstance;

                //Если объект обладает ненулевой ценой перемещения, 
                //то запустим событие по обновлению тайла (для поиска пути)
                if (objInstance.movementCost != 0) {
                    t.InvokeOnChanged();
                }
            }        

        return true;
    }

    public bool RemoveObject()
    {       
        if(installedObject == null) {
            return false;
        }

        Tile t = installedObject.tile;
                
        int w = installedObject.width;
        int h = installedObject.height;

        float objectMoveCost = installedObject.movementCost;

        for (int x_off = t.x; x_off < t.x + w; ++x_off)
            for (int y_off = t.y; y_off < t.y + h; ++y_off)
            {
                Tile t2 = World.current[x_off, y_off];                

                t2.installedObject = null;

                //уведомим об обновлении тайла
                if (objectMoveCost != 0) {
                    t2.InvokeOnChanged();
                }
            }
        
        return true;
    }

    public bool IsNeighbour(Tile t, bool isDiagOK)
    {       
        return (Mathf.Abs(x - t.x) + Mathf.Abs(y - t.y) == 1) || (isDiagOK && Mathf.Abs(t.x - x) == 1 &&  Mathf.Abs(t.y - y) == 1);        
    }

    public Tile[] GetNeighbours(bool diagOK)
    {
        Tile[] ns;

        if(diagOK == false) {
            ns = new Tile[4];   //N E S W
        } else {
            ns = new Tile[8];   //N E S W NE SE SW NW
        }        

        ns[0] = World.current[x, y + 1];
        ns[1] = World.current[x + 1, y];
        ns[2] = World.current[x, y - 1];
        ns[3] = World.current[x - 1, y];

        if (diagOK == false) return ns;

        ns[4] = World.current[x + 1, y + 1];
        ns[5] = World.current[x + 1, y - 1];
        ns[6] = World.current[x - 1, y - 1];
        ns[7] = World.current[x - 1, y + 1];

        return ns;
    }        

    // ------ SAVE AND LOAD    

    public void Write(MiniJSON.JSONObject json)
    {
        json.AddInt("X", x);
        json.AddInt("Y", y);
        json.AddInt("Type", (int)Type);
    }

    public void Read(MiniJSON.JSONObject json)
    {
        Type = (TileType)json.GetInt("Type");
    }
}
