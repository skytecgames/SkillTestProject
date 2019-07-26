using UnityEngine;
using System.Collections.Generic;

public class InstalledObjectSprite_BuildPoint
{
    //TOTHINK: как этот скрипт должен быть связан с InstalledObject?

    public static Sprite GetObjectSprite(InstalledObject obj)
    {
        //TOFIX: тут должна быть анимация процесса строительства
        //       проблема в том, что для этой анимации недостаточно просто возвращать спрайт (нужны дополнительные объекты)        

        return SpriteCollection.GetInstalledObjectSprite(obj.objectType);
    }

    public static void UpdateSprite(InstalledObject obj, SpriteRenderer render)
    {
        //Debug.LogFormat("UpdateSprite scale = {0}:{1}", obj.width, obj.height);
        
        //меняем размеры спрайта
        render.drawMode = SpriteDrawMode.Sliced;
        render.size = new Vector2(obj.width, obj.height);
    }
}
