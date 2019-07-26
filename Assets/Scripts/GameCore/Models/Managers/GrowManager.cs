using UnityEngine;
using System.Collections;

public class GrowManager
{
    //Объект процесса управления ростом
    private static GrowModel instance = null;

    //Ппривязка к процессу
    public static void Init(GrowModel process)
    {
        instance = process;
    }

    //Вызывается при создании нового объекта с параметрами роста
    //Также вызывается при возобновлении процесса проста по какой то причине
    public static void RegisterObject(IParameters obj)
    {
        if (instance == null) {
            Debug.LogError("Error (Process_Grow): try register object before process initialization");
            return;
        }

        instance.RegisterObject(obj);
    }
}
