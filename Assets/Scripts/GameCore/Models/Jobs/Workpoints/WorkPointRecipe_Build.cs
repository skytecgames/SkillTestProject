using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using MiniJSON;

//Класс для описания рецепта для строительства устанавливаемого объекта
public class WorkPointRecipe_Build : WorkPointRecipe
{
    //Тип устанавливаемого объекта
    public string objectType;    

    //Метод загрузки данных из JSON словаря для рецепта строительства
    protected override void LoadJSONData(JSONObject data)
    {
        base.LoadJSONData(data);

        if(data.ContainsKey("objectType") == false) {
            Debug.LogError("WorkPointRecipe_Build error: objectType not found");
            return;
        }

        objectType = data.GetString("objectType");
    }
}
