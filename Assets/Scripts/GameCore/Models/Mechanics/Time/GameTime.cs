using UnityEngine;
using System.Collections;

//Объект для учета игрового времени (учитываем именно время игры)
public class GameTime
{
    public static float value;

    public void Update(float delta)
    {
        value += delta;
    }
}
