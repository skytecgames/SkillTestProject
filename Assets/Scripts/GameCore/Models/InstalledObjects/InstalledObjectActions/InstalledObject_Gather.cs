using UnityEngine;
using System.Collections;

public class InstalledObject_Gather
{
    public static void OnCreate(InstalledObject obj)
    {
        GatherManager.RegisterObject(obj);
    }
}
