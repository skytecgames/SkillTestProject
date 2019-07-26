using UnityEngine;
using System.Collections;

public class RecipeManager
{
    private static WorkPointCollection collection;
    private static WorkbenchCollection workbenches;

    public static void Init()
    {
        collection = new WorkPointCollection();
        collection.Init();

        workbenches = new WorkbenchCollection();
        workbenches.Init();
    }

    //Получить рецепт строительства
    public static WorkPointRecipe_Build GetBuildRecipe(string objectType)
    {
        return collection.GetBuildRecipe(objectType);
    }

    //Получить рецепт производства предмета
    public static WorkPointRecipe_Item GetItemRecipe(string processType)
    {
        return collection.GetItemRecipe(processType);
    }

    //Получить рецепт сбора материалов (получаемых при сборе ресурсов)
    public static WorkPointRecipe_Gather GetGatherRecipe(string objectType)
    {
        return collection.GetGatherRecipe(objectType);
    }

    //Получить настройки производства для станка
    public static WorkbenchInfo GetWorkbenchInfo(string objectType)
    {
        return workbenches.GetInfo(objectType);
    }
}
