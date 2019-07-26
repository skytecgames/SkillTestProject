using UnityEngine;
using System.Collections.Generic;

//Класс предоставляет информацию о предметах
public class ItemInfo
{
    //Возвращает максимальный размер кучи при размещении на земле
    public static int GetMaxStackSize(string itemType)
    {
        //TOFIX: реализовать зависимость от типа предмета
        return 50;
    }
}
