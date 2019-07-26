using UnityEngine;
using System.Collections;

public class GatherManager
{
    private static IGatherFinder finder;
    private static IObjectRegister model;

    //Init
    public static void Init()
    {
        GatherModel obj = new GatherModel();
        finder = obj;
        model = obj;
    }

    //Выдать ближайшую к start точку сбора ресурсов
    public static WorkPoint GetClosestGatherPoint(Tile start)
    {
        return finder.GetClosestGatherPoint(start);
    }

    //Зарегеистрировать объект с которого возможен сбор ресурсов
    public static void RegisterObject(InstalledObject obj)
    {
        model.RegisterObject(obj);
    }
}
