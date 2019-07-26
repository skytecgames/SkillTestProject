using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using MiniJSON;

public class WorkPointRecipe_Item : WorkPointRecipe
{
    //Тип и количество производимых предметов
    public List<KeyValuePair<string, int>> items;

    //Название рецепта для отображения игроку
    //public string processType;

    //Идентификатор рецепта
    public string id;

    protected override void LoadJSONData(JSONObject data)
    {
        base.LoadJSONData(data);

        if (data.ContainsKey("resultItems") == false) {
            Debug.LogError("WorkPointRecipe_Build error: resultItems not found");
            return;
        }
                
        JSONArray items_data = data.GetArray("resultItems");

        if (items_data == null) {
            Debug.LogError("WorkPointRecipe_Build error: resultItems has wrong type");
            return;
        }

        items = new List<KeyValuePair<string, int>>();

        IEnumerator<object> ienum = items_data.GetEnumerator();
        while(ienum.MoveNext()) {
            JSONObject obj = new JSONObject(ienum.Current);
            if(obj.ContainsKey("item") == false || obj.ContainsKey("amount") == false) {
                Debug.LogError("WorkPointRecipe_Build error: resultItems object has wrong format");
                continue;
            }

            KeyValuePair<string, int> item = new KeyValuePair<string, int>(obj.GetString("item"), obj.GetInt("amount"));

            items.Add(item);
        }

        //TODO: remove ProcessType from item recipes
        //processType = data.GetString("objectType");

        //Идентификатор рецепта
        id = data.GetString("id");
    }
}
