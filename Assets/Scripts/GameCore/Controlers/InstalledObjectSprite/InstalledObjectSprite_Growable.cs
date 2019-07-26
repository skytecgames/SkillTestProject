using UnityEngine;
using System.Collections.Generic;

public class InstalledObjectSprite_Growable
{
    //Коллекция имен спрайтов
    private static Dictionary<string, string[]> spriteNames = new Dictionary<string, string[]>();

    public static Sprite GetObjectSprite(InstalledObject obj)
    {
        // [OPTIMISE] Строка с названием спрайта должна кешироваться! 
        // Этот метод вызывается слишком часто, чтобы этого не делать

        //Создаем массив имен для спрайтов
        if(spriteNames.ContainsKey(obj.objectType) == false) {            
            int stage_max = (int)obj.parameters.GetFloat(ParameterName.grow_stage_max);
            spriteNames.Add( obj.objectType, new string[stage_max]);

            for(int i=0;i<stage_max;++i) {
                spriteNames[obj.objectType][i] = string.Format("{0}_G{1}", obj.objectType, i + 1);
            }
        }

        //Берем нужное имя из коллекции по номеру стадии роста        
        string spriteName = spriteNames[obj.objectType][(int)obj.parameters.GetFloat(ParameterName.grow_stage) - 1];        

        return SpriteCollection.GetInstalledObjectSprite(spriteName);
    }

    public static void UpdateSprite(InstalledObject obj, SpriteRenderer render)
    {
        
    }
}
