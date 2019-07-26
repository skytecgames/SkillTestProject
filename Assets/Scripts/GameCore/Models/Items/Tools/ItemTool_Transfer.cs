using UnityEngine;
using System.Collections;

//Утилита для управления передачей предметов между контейнерами
public class ItemTool_Transfer
{
    //Transfer item from source to target container. Return exact amount of transfered items
    public int TransferItem(ItemContainer source, ItemContainer target, string itemType, int amount)
    {
        //calculate amount of items that we can transfer        
        int target_amount = Mathf.Min(target.CanAdd(itemType), source.HasItem(itemType));
        target_amount = Mathf.Min(target_amount, amount);

        //can't transfer any items
        if (target_amount == 0) return 0;

        //pick item from source container
        //TOFIX: тут должна быть работа с зарезервированным для действия предметом
        Item item_to_pick = source.RemoveItem(itemType, target_amount);

        //error when picking item
        if (item_to_pick == null) {
            Debug.LogError("TransferItem: pick null item");
            return 0;
        }

        //разместить предмет в инвентаре персонажа
        target.AddItem(item_to_pick);

        return target_amount;
    }

    //Transfer item from source container to workpoint's material container
    public int TransferItem(ItemContainer source, WorkPoint target, string itemType, int amount)
    {
        int target_amount = Mathf.Min(target.materials.NeedMaterial(itemType), source.HasItem(itemType));
        target_amount = Mathf.Min(target_amount, amount);

        //can't transfer item
        if (target_amount == 0) return 0;

        Item item_to_place = source.RemoveItem(itemType, target_amount);

        if (item_to_place == null) {
            Debug.LogError("TransferMaterial: pick null item");
            return 0;
        }

        //добавляем предмет в контейнер
        target.AddMaterial(item_to_place);

        return target_amount;
    }
}
