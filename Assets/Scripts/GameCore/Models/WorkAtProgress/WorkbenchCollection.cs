using UnityEngine;
using System.Collections.Generic;
using MiniJSON;

public class WorkbenchCollection
{
    private Dictionary<string, WorkbenchInfo> collection;

    //Загрузка рецептов в коллекцию
    public void Init()
    {
        collection = new Dictionary<string, WorkbenchInfo>();

        //Список директорий для поиска в них рецептов
        string[] directories = new string[] { "\\Workbenches\\" };

        //Инициализируем энумератор для текстов рецептов
        TextFileEnumerator enumerator = new TextFileEnumerator(directories);
        IEnumerator<string> jsonEnum = enumerator.GetEnumerator();

        //Перебираем найденные тексты
        while (jsonEnum.MoveNext()) {            
            JSONObject json = JSONObject.Parse(jsonEnum.Current);

            //если не рецепт станка, пропускаем
            if(json.GetString("RecipeType") != "Workbench") {
                continue;
            }

            //Инициализуруем хранилище данных для рецепта
            WorkbenchInfo i = new WorkbenchInfo();
            string objectType = json.GetString("objectType");

            //Читаем идентификатор типа очереди производства
            i.queue_id = json.GetString("queueType");

            //Читаем список рецептов
            JSONArray recipies = json.GetArray("recipes");
            List<string> r = new List<string>();
            foreach(object o in recipies) {
                JSONObject recipe_info = new JSONObject(o);
                r.Add(recipe_info.GetString("id"));
            }

            i.recipes = r;

            //Добавляем элемент в коллекцию
            collection.Add(objectType, i);            
        }        
    }

    //Возвращает настройки станка по идентификатору
    public WorkbenchInfo GetInfo(string id)
    {
        if (collection.ContainsKey(id)) return collection[id];

        return null;
    }
}