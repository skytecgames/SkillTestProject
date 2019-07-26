using UnityEngine;
using System.Collections.Generic;

//Очередь производства циклически переключает рецепты в списке
public class WorkQueue_Repeat : IWorkQueue
{
    //Список рецептов в очереди
    private List<WorkPointRecipe_Item> recipes;

    //Индекс текущего рецепта
    private int index_current;

    //Текущие настройки
    private WorkPointInfo work_info;

    public WorkQueue_Repeat(List<string> recipes)
    {
        this.recipes = new List<WorkPointRecipe_Item>();
        index_current = -1;

        for (int i = 0; i < recipes.Count; ++i) {
            AddRecipe(RecipeManager.GetItemRecipe(recipes[i]));
        }

        work_info = new WorkPointInfo();
    }

    public WorkPointInfo Current()
    {
        return work_info;
    }

    public WorkPointInfo Next()
    {
        index_current++;
        if (index_current >= recipes.Count) index_current = 0;
        
        recipes[index_current].GetInfo(work_info);

        return work_info;
    }

    public List<KeyValuePair<string, int>> Product()
    {
        return recipes[index_current].items;
    }

    //Добавляет рецепт в очередь на производство
    public void AddRecipe(WorkPointRecipe_Item recipe)
    {
        recipes.Add(recipe);
    }    
}
