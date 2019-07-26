using UnityEngine;
using System.Collections.Generic;
using System.Text;

//Класс для хранения материалов (содержит информацию о текущем и необходимом количестве предметов)
//не визуализирует себя сам (отображение для этого класса сделано через отображение WorkPoint)
public class MaterialContainer
{
    //Информация о материалах
    private Dictionary<string, Item> list = new Dictionary<string, Item>();

    //Тайл на котором расположен контейнер (тот же тайл что и тайл WorkPoint)
    public Tile tile;

    //TOTHINK: так как контейнер материалов работает только в связке с WorkPoint, то не имеет смысла здесь
    //         создавать события. Или есть?

    //проверить контейнер на наличие всех материалов
    public bool HasAllMaterials()
    {
        //Если хотя бы одного материала не хватает, возвращаем false
        foreach(string s in list.Keys) {
            if (list[s].stackSize < list[s].maxStackSize) return false;
        }
        
        return true;
    }

    //Возвращает, сколько предметов указанного типа не хватает
    public int NeedMaterial(string itemType)
    {
        if(list.ContainsKey(itemType)) {
            return list[itemType].maxStackSize - list[itemType].stackSize;
        }

        return 0;
    }

    //Метод позволяет получить наименование всех материалов необходимых для работы
    public List<string> GetMaterialTypes()
    {        
        return new List<string>(list.Keys);
    }

    //Добавить материал в контейнер
    public void AddMaterial(Item item)
    {
        //Если нам нужен такой материал то добавляем его
        if(list.ContainsKey(item.objectType)) {
            list[item.objectType].stackSize += item.stackSize;
            
            if(list[item.objectType].stackSize > list[item.objectType].maxStackSize) {
                Debug.LogError("MaterialContainer.AddMaterial: too many materials added");
            }

            Debug.LogFormat("MaterialAdded: {1}({2}), {0}", ToString(), item.objectType, item.stackSize);

            return;
        }

        Debug.Log("MaterialContainer.AddMaterial: wrong material type");
    }

    //Потратить материалы из контейнера (1 стак)
    public void Spend()
    {
        //Перебираем все материалы и тратим количество необходимое для одного цикла производства
        foreach (string s in list.Keys) {
            list[s].stackSize -= list[s].maxStackSize;            
        }
    }

    //опустошить контейнер с материалами
    public void Empty()
    {
        Debug.LogFormat("MaterialContainer.Empty {0}", ToString());

        //Перебираем все материалы и выбрасываем их на пол
        foreach(string s in list.Keys) {
            if (list[s].stackSize > 0) {
                //ItemManager.Instance.SpawnItem(tile, list[s]);
                ItemManagerA.SpawnItem(tile, list[s].objectType, list[s].stackSize);
            }
        }

        list.Clear();

        Debug.Log("MaterialContainer.Empty");
    }

    //Инициализация контейнера списком указанных материалов
    public void SetMaterials(List<KeyValuePair<string, int>> materials_list)
    {       
        //Убираем из контейнера все оставшиеся в нем материалы
        Empty();

        //Наполняем список материалов
        for(int i = 0; i < materials_list.Count; ++i) {
            Item item = new Item(materials_list[i].Key, 0, materials_list[i].Value);
            list.Add(item.objectType, item);
        }

        Debug.LogFormat("MaterialSetted: {0}", ToString());
    }

    public override string ToString()
    {
        StringBuilder sb = new StringBuilder();

        sb.Append("MaterialContainer");

        if (tile != null) {
            sb.AppendFormat(" [{0}:{1}]", tile.x, tile.y);
        }

        sb.Append(" {");

        int i = 0;

        foreach(string k in list.Keys) { 
            sb.AppendFormat("{3}{0}({1}/{2})", k, list[k].stackSize, list[k].maxStackSize, i == 0 ? "" : ",");
            i++;
        }

        sb.Append("}");

        return sb.ToString();
    }
}
