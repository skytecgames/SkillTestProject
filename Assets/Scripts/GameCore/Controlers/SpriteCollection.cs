using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SpriteCollection : MonoBehaviour
{
    public Sprite floorSprite;
    public Sprite emptySprite;

    public Sprite wallSprite;

    static SpriteCollection instance;
    //Коллекция спрайтов для тайлов
    Dictionary<int, Sprite> sprites = new Dictionary<int, Sprite>();
    //Коллекция спрайтов для устанавливаемых объектов
    Dictionary<string, Sprite> installedObjectSprites = new Dictionary<string, Sprite>();

    private void OnEnable()
    {
        Sprite[] io_sprites = Resources.LoadAll<Sprite>("Images/InstalledObjects");
        foreach(Sprite s in io_sprites)
        {
            //Debug.LogFormat("Loaded resource [{0}]", s.name);
            installedObjectSprites.Add(s.name, s);
        }


        instance = this;
    }

    public static Sprite GetSprite(int type)
    {
        if (instance.sprites.ContainsKey(type)) {
            return instance.sprites[type];
        }

        if (type == 0)
        {
            instance.sprites.Add(0, instance.emptySprite);
            return instance.emptySprite;
        }

        if (type == 1)
        {
            instance.sprites.Add(1, instance.floorSprite);
            return instance.floorSprite;
        }        

        return null;
    }

    public static Sprite GetInstalledObjectSprite(string objectType)
    {
        if(instance.installedObjectSprites.ContainsKey(objectType) == false)
        {
            Debug.LogError("SpriteCollection: can't find installed object sprite: " + objectType);
            return null;
        }

        return instance.installedObjectSprites[objectType];
    }

    public static bool IsInstalledObjectSpriteValid(string objectType)
    {        
        return instance.installedObjectSprites.ContainsKey(objectType);
    }
}
