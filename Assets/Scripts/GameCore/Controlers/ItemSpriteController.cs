using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class ItemSpriteController : MonoBehaviour
{
    public GameObject ItemUIPrefab;

    //соответствие объекта на карте и предмета
    //Dictionary<Item, GameObject> itemGameObjectMap;
    Dictionary<ItemContainer, GameObject> itemGameObjectMap;

    Dictionary<string, Sprite> itemSprites;

    World world
    {
        get { return World.current; }
    }

    // Use this for initialization
    void Start()
    {
        LoadSprites();

        //itemGameObjectMap = new Dictionary<Item, GameObject>();
        itemGameObjectMap = new Dictionary<ItemContainer, GameObject>();

        //world.cbItemCreated += OnItemCreated;
        world.cbItemContainerCreated += OnItemCreated;

        // Check for pre-existing items, which won't do the callback.        
        foreach (ItemContainer it in ItemManager.Instance.itemContainers)
        {
            OnItemCreated(it);
        }         
    }

    void OnItemCreated(ItemContainer obj)
    {        
        //Контейнер не прикреплен к тайлу и не отображается на игровом поле
        if (obj.tile == null) {            
            Debug.LogError("ItemSpriteController.OnItemCreated tile is null");
            return;
        }        

        //Создаем объект для отображения и добавляем его в карту
        GameObject go = new GameObject();
        itemGameObjectMap.Add(obj, go);        
        
        //Размещаем объект для отображения в пространстве
        go.transform.position = new Vector3(obj.tile.x, obj.tile.y, 0);
        go.transform.SetParent(this.transform, true);

        //Добавляем спрайт
        SpriteRenderer go_sr = go.AddComponent<SpriteRenderer>();        
        go_sr.sortingLayerName = "Item";
        
        //нужно добавьть UI элемент для отображения числа единиц в стаке
        GameObject go_ui = Instantiate(ItemUIPrefab, go.transform);
        go_ui.transform.localPosition = Vector3.zero;        

        //Add change callback
        obj.cbContainerChanged += OnItemChanged;

        //Вызываем обработчик для корректировки отображения
        OnItemChanged(obj);
    }
        
    void OnItemChanged(ItemContainer obj)
    {
        //Correcting link graphics
        if (itemGameObjectMap.ContainsKey(obj) == false) {
            Debug.LogError("OnItemChanged cant find gameObject for item");
            return;
        }

        //Получаем объекты с графикой
        GameObject go = itemGameObjectMap[obj];
        SpriteRenderer go_sr = go.GetComponent<SpriteRenderer>();
        Text go_txt = go.GetComponentInChildren<Text>();

        //Получаем предмет сверху контейнера (на основе него мы будем строить отображение)
        Item top_item = obj.GetTopItem();

        //Если контейнер пустой, то скрываем предмет
        if(top_item == null) {
            go_sr.sprite = null;
            go_txt.text = "";
            go.name = string.Format("Container[{0}:{1}]", obj.tile.x, obj.tile.y);
            return;
        }

        //Изменяем имя, так чтобы было видно какой предмет внутри
        go.name = string.Format("Container[{0}:{1}]({2})", obj.tile.x, obj.tile.y, top_item.objectType);

        //Меняем спрайт
        go_sr.sprite = GetItemSprite(top_item.objectType);

        //нужно изменить UI элемент для отображения числа единиц в стаке                    
        if (top_item.stackSize > 0 && top_item.maxStackSize > 1) {                        
            go_txt.text = top_item.stackSize.ToString();
        } else {
            go_txt.text = "";
        }    
    }    

    Sprite GetItemSprite(string spriteName)
    {        
        return itemSprites[spriteName];
    }

    void LoadSprites()
    {
        itemSprites = new Dictionary<string, Sprite>();

        Sprite[] sprites = Resources.LoadAll<Sprite>("Images/Items");

        foreach (Sprite s in sprites)
        {
            itemSprites.Add(s.name, s);
        }
    }
}
