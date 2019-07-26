using UnityEngine;
using System.Collections.Generic;

public class WorkPointInfo_CreateItem
{
    //Словарь с настройками станков для производства предметов
    private static Dictionary<string, WorkPointInfo> collection = new Dictionary<string, WorkPointInfo>();

    //Возвращает настройки производства для станка (InstalledObject) указанного типа
    public static WorkPointInfo Load(string objectType)
    {
        //TOFIX: производимый предмет и материалы сейчас привязаны к типу станка, а должны к выбранному рецепту

        //Если у нас уже есть загруженные данные по таком типу объектов, то отдаем их
        if (collection.ContainsKey(objectType)) {
            return collection[objectType];
        }

        //Загружаем данные рецепта
        WorkPointRecipe_Item recipe = RecipeManager.GetItemRecipe(objectType);

        //Создаем настройки для точки работы
        WorkPointInfo info = new WorkPointInfo();
        recipe.GetInfo(info);
        info.OnComplete = (workpoint) => { OnComplete(recipe.items, workpoint); };       
        info.OnStart = OnStart;
        info.OnChanged = OnChanged;
        info.OnCancel = OnCancel;

        //сохраняем настройки в коллекции
        collection.Add(objectType, info);

        return info;
    }

    //Обработчик событие - начало производства
    //TOTHINK: это однократно при установке станка или в начале производства каждого предмета???
    private static void OnStart(WorkPoint workpoint)
    {
    }

    //Обработчик события - изменение прогресса производства
    private static void OnChanged(WorkPoint workpoint)
    {
        //TOFIX: анимация производства предмета
    }

    //Обработчик события - отмена производства
    private static void OnCancel(WorkPoint workpoint)
    {
        //TODO: отмена производства предмета (понять кто вызывает)
    }

    //Обработчик события - завершение производства
    private static void OnComplete(List<KeyValuePair<string, int>> items, WorkPoint workpoint)
    {        
        //Спавним полученные предметы
        for(int i=0;i<items.Count;++i) {            
            ItemManagerA.SpawnItem(workpoint.spawnPoint, items[i].Key, items[i].Value);
        }
    }
}
