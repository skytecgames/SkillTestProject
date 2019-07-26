using UnityEngine;
using System.Collections;
using System;
using System.Xml;
using MiniJSON;

public class Character
{
    //координаты персонажа в мировом пространстве
    public float x {
        get {
            return Mathf.Lerp(currTile.x, nextTile.x, movePercentage);
        }
    }

    public float y {
        get {
            return Mathf.Lerp(currTile.y, nextTile.y, movePercentage);
        }
    }

    //текущий, следующий и целевой тайл персонажа
    public Tile currTile;// { get; private set; }
    public Tile nextTile;

    //Агент для персонажа (структура представляющая персонажа как работника для задачи)
    public Agent agent;

    //какую часть пути от текущейго до следующего тайла прошел персонаж
    public float movePercentage;    

    //скорость перемещения персонажа
    public float speed = 8f;    

    //событие на изменение состояния персонажа (перемещение например)
    public System.Action<Character> cbCharacterChanged;

    //предмет в данный момент переносимый персонажем    
    public ItemContainer itemContainer;

    public Character(Tile tile)
    {        
        //задаем позицию персонажа
        currTile = nextTile = tile;

        //Создаем инвентарь персонажа
        itemContainer = new ItemContainer();        
    }

    public void Update(float deltaTime)
    {        
        //Обновляем выполняемую задачу
        agent.Update(deltaTime);

        //TOFIX: обновлять персонажа нужно только если что то в нем изменилось
        if(cbCharacterChanged != null) {
            cbCharacterChanged(this);
        }
    }        

    // ------ SAVE AND LOAD    

    public void Write(JSONObject json)
    {
        json.Add("X", currTile.x);
        json.Add("Y", currTile.y);
    }

    public void Read(JSONObject json)
    {

    }
}
