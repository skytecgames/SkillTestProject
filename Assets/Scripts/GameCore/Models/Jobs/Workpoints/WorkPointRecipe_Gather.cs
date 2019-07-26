using UnityEngine;
using System.Collections.Generic;
using MiniJSON;

//TOTHINK: класс WorkPointRecipe_Gather точная копия WorkPointRecipe_Item
//         но методика загрузки рецептов заставляет меня эту копию создать, можно ли этого избежать?
//         Вообще не проще ли оставить один класс для рецептов и просто добавить в него тип как параметр?

public class WorkPointRecipe_Gather : WorkPointRecipe
{
    //Тип и количество производимых предметов
    public List<KeyValuePair<string, int>> items;

    //Название рецепта для отображения игроку
    public string objectType;

    protected override void LoadJSONData(JSONObject data)
    {
        base.LoadJSONData(data);

        if (data.ContainsKey("resultItems") == false) {
            Debug.LogError("WorkPointRecipe_Gather error: resultItems not found");
            return;
        }

        JSONArray items_data = data.GetArray("resultItems");

        if (items_data == null) {
            Debug.LogError("WorkPointRecipe_Gather error: resultItems has wrong type");
            return;
        }

        items = new List<KeyValuePair<string, int>>();

        IEnumerator<object> ienum = items_data.GetEnumerator();
        while (ienum.MoveNext()) {
            JSONObject obj = new JSONObject(ienum.Current);
            if (obj.ContainsKey("item") == false || obj.ContainsKey("amount") == false) {
                Debug.LogError("WorkPointRecipe_Gather error: resultItems object has wrong format");
                continue;
            }

            KeyValuePair<string, int> item = new KeyValuePair<string, int>(obj.GetString("item"), obj.GetInt("amount"));

            items.Add(item);
        }

        objectType = data.GetString("objectType");
    }
}
