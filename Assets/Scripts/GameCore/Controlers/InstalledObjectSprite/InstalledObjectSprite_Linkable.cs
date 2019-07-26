using UnityEngine;
using System.Collections.Generic;

public class InstalledObjectSprite_Linkable
{
    //Коллекция имен спрайтов
    private static Dictionary<string, Dictionary<int, string>> spriteNames = new Dictionary<string, Dictionary<int, string>>();

    //Вычисление имени спрайта с учетом соседних объектов
    public static Sprite GetObjectSprite(InstalledObject obj)
    {
        // [OPTIMISE] Строка с названием спрайта должна кешироваться! 
        // Этот метод вызывается слишком часто, чтобы этого не делать

        World world = World.current;

        //Создаем массив имен для спрайтов
        if (spriteNames.ContainsKey(obj.objectType) == false) {            
            spriteNames.Add(obj.objectType, new Dictionary<int, string>());

            for(int i = 0; i < 16; ++i) { 
                string name = obj.objectType + "_";

                if ((i & (1 << 0)) > 0) { name += "N"; }
                if ((i & (1 << 1)) > 0) { name += "E"; }
                if ((i & (1 << 2)) > 0) { name += "S"; }
                if ((i & (1 << 3)) > 0) { name += "W"; }

                //Debug.LogFormat("NewName {0} == {1}", i, name);

                spriteNames[obj.objectType].Add(i, name);                
            }
        }        

        //Вычисляем индекс нужного имени в зависимости от наличия соседей
        int index = 0;

        if (obj.tile != null) {
            Tile t;
            int x = obj.tile.x;
            int y = obj.tile.y;

            t = world[x, y + 1];
            if (t != null && t.installedObject != null && t.installedObject.objectType == obj.objectType) {                
                index |= 1 << 0;
            }

            t = world[x + 1, y];
            if (t != null && t.installedObject != null && t.installedObject.objectType == obj.objectType) {                
                index |= 1 << 1;
            }

            t = world[x, y - 1];
            if (t != null && t.installedObject != null && t.installedObject.objectType == obj.objectType) {                
                index |= 1 << 2;
            }

            t = world[x - 1, y];
            if (t != null && t.installedObject != null && t.installedObject.objectType == obj.objectType) {                
                index |= 1 << 3;
            }
        }

        string spriteName = spriteNames[obj.objectType][index];        

        return SpriteCollection.GetInstalledObjectSprite(spriteName);
    }    
}
