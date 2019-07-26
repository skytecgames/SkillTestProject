using UnityEngine;
using System.Collections;

public class InstalledObject_Grow
{
    public static void OnCreate(InstalledObject obj)
    {
        GrowManager.RegisterObject(obj.parameters);
    }
}
