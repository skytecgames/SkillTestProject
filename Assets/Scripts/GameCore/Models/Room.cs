using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class Room
{
    public static Room OutsideRoom = null;

    //состав атмосферы в комнате
    Dictionary<string, float> atmosphericGasses = new Dictionary<string, float>();

    //тайлы входящие в комнату
    List<Tile> tiles = new List<Tile>();

    public void AssignTile(Tile t)
    {
        if (tiles.Contains(t))
        {
            return;
        }

        if(t.room != null) {
            t.room.tiles.Remove(t);
        }

        t.room = this;        
        tiles.Add(t);        
    }

    public void UnassignAllTiles()
    {
        for(int i=0; i<tiles.Count;++i)
        {
            tiles[i].room = Room.OutsideRoom;
        }

        tiles = new List<Tile>();
    }    

    public static void DoRoomFloodFill(Tile sourceTile)
    {
        Room oldRoom = sourceTile.room;

        if (oldRoom != null)
        {
            //Определяем комнаты со всех сторон
            Tile[] neighbours = sourceTile.GetNeighbours(false);
            foreach (Tile t in neighbours)
            {
                ActualFloodFill(t, oldRoom);
            }

            //Этот тайл разделитель и не может быть частью комнаты
            sourceTile.room = null;
            oldRoom.tiles.Remove(sourceTile);

            if (oldRoom.tiles.Count > 0)
            {
                Debug.LogError("old room still has tiles!!!");
            }

            World.current.DeleteRoom(oldRoom);
        } else {
            ActualFloodFill(sourceTile, null);
        }
    }

    protected static void ActualFloodFill(Tile tile, Room oldRoom)
    {        
        if(tile == null) {
            return;
        }

        //Проверим что тайл не обработан уже и не приписан к другой комнате
        if(tile.room != oldRoom) {
            return;
        }

        //Тайл является ограничителем комнаты и не может быть частью комнаты
        if(tile.installedObject != null && tile.installedObject.parameters.GetFloat(ParameterName.room_enclosure) == 1f) {
            return;
        }

        if(tile.Type == TileType.Empty) {
            return;
        }

        //Создаем комнату
        Room newRoom = new Room();

        //Создаем список тайлов для комнаты
        Queue<Tile> tilesToCheck = new Queue<Tile>();
        tilesToCheck.Enqueue(tile);

        bool isConnectedToOutside = false;
        int processedTile = 0;
        List<Room> checked_rooms = new List<Room>();

        while(tilesToCheck.Count > 0) {
            Tile t = tilesToCheck.Dequeue();

            processedTile++;

            //if(t.room == oldRoom) {
            if (t.room != newRoom) {
                newRoom.AssignTile(t);

                Tile[] neighbours = t.GetNeighbours(false);

                foreach(Tile t2 in neighbours) {                    
                    if(t2 == null || t2.Type == TileType.Empty) {
                        //it is outside room                        

                        if (oldRoom != null) {
                            newRoom.UnassignAllTiles();
                            return;
                        }

                        isConnectedToOutside = true;
                        continue;
                    }

                    if (t2.room != newRoom && (t2.installedObject == null || t2.installedObject.parameters.GetFloat(ParameterName.room_enclosure) == 0)) {
                        if(checked_rooms.Contains(t2.room) == false) {
                            checked_rooms.Add(t2.room);
                        }
                        tilesToCheck.Enqueue(t2);
                    }
                }
            }
        }

        Debug.LogFormat("ProcessedTile: {0}", processedTile);

        if (oldRoom == null) {
            Debug.LogFormat("CheckedRooms: {0}", checked_rooms.Count);
            foreach(Room r in checked_rooms) {
                World.current.DeleteRoom(r);
            }
        }

        if(isConnectedToOutside) {
            newRoom.UnassignAllTiles();
            return;
        }

        //пересчитываем параметры комнаты (аля кислород)        
        if (oldRoom != null) {
            newRoom.CopyGasses(oldRoom);
        } else {
            //TODO: нужно смешать газ в комнатах
        }

        World.current.AddRoom(newRoom);
    }

    // ------ функции для управления отмосферой в комнате ------

    public void ChangeGas(string gas, float amount)
    {
        //мы не можем поменять состав атмосферы за бортом станции, там всегда вакуум
        if(this == OutsideRoom) {
            return;
        }

        if(atmosphericGasses.ContainsKey(gas) == false) {
            atmosphericGasses.Add(gas, 0);
        }

        atmosphericGasses[gas] += amount;

        if(atmosphericGasses[gas] < 0) {
            atmosphericGasses[gas] = 0;
        }
    }

    public float GetGasAmount(string gas)
    {
        if(atmosphericGasses.ContainsKey(gas)) {
            return atmosphericGasses[gas];
        }

        return 0;
    }

    public float GetGasPercentage(string gas)
    {
        if(atmosphericGasses.ContainsKey(gas) == false) {
            return 0;
        }

        float t = 0;

        foreach(float g in atmosphericGasses.Values) {
            t += g;
        }

        return atmosphericGasses[gas] / t;
    }

    public string[] GetGasNames() {
        return atmosphericGasses.Keys.ToArray();
    }

    protected void CopyGasses(Room other)
    {
        foreach(string k in other.atmosphericGasses.Keys) {
            this.atmosphericGasses[k] = other.atmosphericGasses[k];
        }
    }

    // ---------------------------------------------------------
}
