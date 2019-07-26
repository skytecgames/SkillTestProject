using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Text;

public class WorkPointCollection
{
    //Рецепты для устанавливаемых объектов
    private IDictionary<string, WorkPointRecipe_Build> recipes_build;

    //Рецепты для предметов
    private IDictionary<string, WorkPointRecipe_Item> recipes_item;

    //Рецепты сбора ресурсов
    private IDictionary<string, WorkPointRecipe_Gather> recipes_gather;

    //TOTHINK: хорошее место для того, чтобы подумать о том, как уменьшить фрагментацию памяти после выполнения
    //         этого метода. В идеале тут нужно создать один буффер для чтения в него данных и потом не создавая 
    //         его копий распарсить его

    //Загрузка всех рецептов
    public void Init()
    {
        List<string> files_list = new List<string>();
        string[] files = null;

        string[] directories = new string[] { "\\InstalledObjects\\", "\\Items\\", "\\Gather\\" };

        // поиск файлов с рецептами
        for (int i = 0; i < directories.Length; ++i) {
            files = System.IO.Directory.GetFiles(Application.streamingAssetsPath + directories[i],
            "*.txt", SearchOption.TopDirectoryOnly);
            if (files != null) files_list.AddRange(files);
        }        

        if (files_list.Count == 0) return;

        recipes_build = new Dictionary<string, WorkPointRecipe_Build>();
        recipes_item = new Dictionary<string, WorkPointRecipe_Item>();
        recipes_gather = new Dictionary<string, WorkPointRecipe_Gather>();

        for(int i = 0; i < files_list.Count; ++i) {
            Debug.LogFormat("Recipe file={0}", files_list[i]);
            string json = File.ReadAllText(files_list[i]);

            Debug.LogFormat("json={0}", json);

            WorkPointRecipe recipe = WorkPointRecipe.LoadFromJSON(json);

            if (recipe is WorkPointRecipe_Build) {
                WorkPointRecipe_Build recipe_build = (WorkPointRecipe_Build)recipe;
                recipes_build.Add(recipe_build.objectType, recipe_build);
            }

            if(recipe is WorkPointRecipe_Item) {
                WorkPointRecipe_Item recipe_item = recipe as WorkPointRecipe_Item;
                recipes_item.Add(recipe_item.id, recipe_item);
            }

            if(recipe is WorkPointRecipe_Gather) {
                WorkPointRecipe_Gather recipe_gather = recipe as WorkPointRecipe_Gather;
                recipes_gather.Add(recipe_gather.objectType, recipe_gather);
            }
        }
    }

    //Возвращает рецепт строительства для указанного устанавливаемого объекта
    //TOFIX: идентификатором устанавливаемого объекта должно выступать число
    public WorkPointRecipe_Build GetBuildRecipe(string objectType)
    {
        if(recipes_build.ContainsKey(objectType)) {
            return recipes_build[objectType];
        }

        return null;
    }

    //Возвращает рецепт для производства предмета по указанному названию рецепта
    //TOFIX: [OPTIMISE] идентификатором рецепта должно выступать число
    //TOFIX: идентификатор предмета не должен быть привязан к станку
    public WorkPointRecipe_Item GetItemRecipe(string processType)
    {
        if(recipes_item.ContainsKey(processType)) {
            return recipes_item[processType];
        }

        return null;
    }

    //Возвращает рецепт для механики сбора ресурсов
    //TOFIX: [OPTIMISE] идентификатором рецепта должно выступать число
    //TOFIX: идентификатор предмета не должен быть привязан к станку
    public WorkPointRecipe_Gather GetGatherRecipe(string objectType)
    {
        if(recipes_gather.ContainsKey(objectType)) {
            return recipes_gather[objectType];
        }

        return null;
    }
}
