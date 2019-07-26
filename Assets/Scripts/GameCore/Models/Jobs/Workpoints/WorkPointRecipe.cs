using UnityEngine;
using System.Collections.Generic;
using MiniJSON;

public class WorkPointRecipe
{    
    //прототип для точки работы (по сути содержит информацию о затратах на создание)
    protected WorkPointInfo prototype;

    // README
    // мы не можем представить результат работы универсальным набором данных, так как
    // такой набор будет избыточным. По сути результат нашей работы это функция OnComplete.
    // Для устанавливаемых объектов она будет иметь один вид, для предметов другой.
    // Для устанавливаемых объектов мы должны хранить только тип получаемого объекта, для
    // предметов нужно тип предмета и количество. Поэтому лучше рецепты для объектов и для
    // предметов представлять разными классами

    //Создает объект WorkPointInfo согласно рецепту
    public void GetInfo(WorkPointInfo info)
    {                        
        //Загружаем данные о затратах на выполнение задачи
        info.workAmount = prototype.workAmount;      
        //передаем материалы без создания копии
        info.materials = prototype.materials;
    }

    //Загружает данные рецепта из JSON и создает объект соответствующего типа
    //Этот метод по сути инкапсулированная в базовый класс рецепта фабрика объектов
    public static WorkPointRecipe LoadFromJSON(string json)
    {        
        JSONObject obj = JSONObject.Parse(json);

        //Если в результате парсинга у нас получился не словарь, то рецепт неверный
        if (obj == null) {
            Debug.LogError("WorkPointRecipe.LoadFromJSON error: wrong json data");
            return null;
        }        

        //Если в json нет параметра RecipeType указывающего на то, какой тип рецепта мы должны загрузить, то это ошибка
        if(obj.ContainsKey("RecipeType") == false) {
            Debug.LogError("WorkPointRecipe.LoadFromJSON error: json data does not contain RecipeType");
            return null;
        }

        //Создаем экземпляр рецепта согласно его типу        
        string type = obj.GetString("RecipeType");
        WorkPointRecipe result = null;        
        switch(type) {
            case "Build":
                result = new WorkPointRecipe_Build();
                break;
            case "Item":
                result = new WorkPointRecipe_Item();
                break;
            case "Gather":
                result = new WorkPointRecipe_Gather();
                break;
            default:
                Debug.LogErrorFormat("WorkPointRecipe.LoadFromJSON error: RecipeType {0} is unknown", type);
                return null;
        }

        //Загружаем данные рецепта
        result.LoadJSONData(obj);

        return result;
    }

    //Метод загрузки данных из JSON словаря (для конкретного типа рецепта)
    protected virtual void LoadJSONData(JSONObject data)
    {
        //проверяем наличие параметров в данных
        if(data.ContainsKey("work_amount") == false) {
            Debug.LogError("WorkPointRecipe error: json does not containt work_amount param");
            return;
        }
        if(data.ContainsKey("materials") == false) {
            Debug.LogError("WorkPointRecipe error: json does not containt materials param");
            return;
        }

        prototype = new WorkPointInfo();

        //загружаем данные                
        prototype.workAmount = (float)data.GetNumber("work_amount");
                
        //загружаем данные о материалах, если они есть
        JSONArray mats = data.GetArray("materials");        
        prototype.materials = new List<KeyValuePair<string, int>>();

        //Загружаем материалы        
        if (mats != null) {
            IEnumerator<object> ienum = mats.GetEnumerator();
            while(ienum.MoveNext()) {
                JSONObject mat_obj = new JSONObject(ienum.Current);
                if (mat_obj.ContainsKey("item") == false || mat_obj.ContainsKey("amount") == false) {
                    Debug.LogError("WorkPointRecipe error: materials value in wrong format");
                    continue;
                }

                KeyValuePair<string, int> item = new KeyValuePair<string, int>(mat_obj.GetString("item"), mat_obj.GetInt("amount"));

                prototype.materials.Add(item);
            }
        }
    }    
}
