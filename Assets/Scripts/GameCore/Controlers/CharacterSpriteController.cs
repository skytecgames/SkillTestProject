using UnityEngine;
using System.Collections.Generic;

public class CharacterSpriteController : MonoBehaviour
{    
    Dictionary<Character, GameObject> characterGameObjectMap;

    Dictionary<string, Sprite> characterSprites;

    World world {
        get { return World.current; }
    }

    // Use this for initialization
    void Start()
    {
        LoadSprites();

        characterGameObjectMap = new Dictionary<Character, GameObject>();

        world.cbCharacterCreated += OnCharacterCreated;

        // Check for pre-existing characters, which won't do the callback.
        foreach (Character c in world.characters)
        {
            OnCharacterCreated(c);
        }

        //TEST
        //Character c = world.CreateCharacter(world[world.width / 2, world.height / 2]);        
    }

    void OnCharacterCreated(Character obj)
    {
        GameObject go = new GameObject();

        characterGameObjectMap.Add(obj, go);

        go.name = "Character";
        go.transform.position = new Vector3(obj.x, obj.y, 0);
        go.transform.SetParent(this.transform, true);

        SpriteRenderer go_sr = go.AddComponent<SpriteRenderer>();
        go_sr.sprite = GetCharacterSprite("character");        
        go_sr.sortingLayerName = "Character";

        obj.cbCharacterChanged += OnCharacterChanged;
    }

    void OnCharacterChanged(Character obj)
    {
        //Correcting link graphics
        if (characterGameObjectMap.ContainsKey(obj) == false)
        {
            Debug.LogError("OnCharacterChanged cant find gameObject for character");
            return;
        }

        //characterGameObjectMap[obj].GetComponent<SpriteRenderer>().sprite = GetInstalledObjectSprite(obj);

        characterGameObjectMap[obj].transform.position = new Vector3(obj.x, obj.y, 0);
    }

    Sprite GetCharacterSprite(string spriteName)
    {
        return characterSprites[spriteName];
    }

    void LoadSprites()
    {
        characterSprites = new Dictionary<string, Sprite>();

        Sprite[] sprites = Resources.LoadAll<Sprite>("Images/Characters");

        foreach(Sprite s in sprites) {
            characterSprites.Add(s.name, s);
        }
    }
}
