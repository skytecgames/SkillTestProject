using UnityEngine;
using System.Collections.Generic;
using System.Text;

//Класс для хранения предметов 
//(содержит правила для ограничения разменщения в нем предметов, количество и тип предметов, может быть визуализирован)
//TOFIX: этот класс должен быть интерфейсом так как конкретная реализация у него может быть разной
//       в зависимости от типа контейнера
public class ItemContainer
{
    //Список в котором мы будем хранить содержимое контейнера
    private List<Item> list = new List<Item>(1);

    //событие уведомляющее об изменении состояния контейнера
    //TOTHINK: проблема этого события в том, что оно не говорит нам как изменился контейнер (стоит ли сделать другой эвент?)
    //         У нас есть не так много методов для изменения содержимого контейнера
    public System.Action<ItemContainer> cbContainerChanged;

    //событие уведомляющее об удалении контейнера    
    public System.Action<ItemContainer> cbContainerRemoved;

    //Событие уведомляющее о добавлении/удалении предмета
    public System.Action<ItemContainer, string, int> cbContainerItemChanged;    

    //Тайл к которому прикреплен контейнер, если такой есть
    //null, если контейнер не закреплен (например если это инвентарь персонажа)
    public Tile tile;
    
    public ItemContainer() { }

    public Item GetTopItem()
    {
        if(list.Count == 0) {
            return null;
        }

        //Берем первый непустой предмет и отдаем его
        for(int i=0;i<list.Count;++i) {
            if (list[i].stackSize > 0) return list[i];
        }

        return null;
    }

    //Список типов предметов в контейнере
    public List<string> GetItemTypes()
    {
        List<string> result = new List<string>();
        for(int i=0;i<list.Count;++i) {
            if(list[i].stackSize > 0) {
                result.Add(list[i].objectType);
            }
        }

        return result;
    }

    //Количество разных типлв предметов в контейнере
    public int GetItemTypesCount()
    {
        //TOTHINK: нужно ли проверять что стаки в списке не пустые?

        return list.Count;
    }

    //функция для добавления предмета в контейнер. Не контролирует размеры контейнера.
    //Перед вызовом этого метода нужно проверить контейнер на возможность размещать в нем указанный предмет
    //в указанном количестве
    public void AddItem(Item item)
    {
        AddItem(item.objectType, item.stackSize);

        //удаляем предмет из источника
        item.stackSize = 0;
    }

    //функция для добавления предмета в контейнер. Не контролирует размеры контейнера.
    //Перед вызовом этого метода нужно проверить контейнер на возможность размещать в нем указанный предмет
    //в указанном количестве
    public void AddItem(string objectType, int amount)
    {
        //Проверка на то, что предмет влезает в контейнер (выдает ошибку, но не препятствует размещению)
        if (CanAdd(objectType) < amount) {
            Debug.LogError("ItemContainer.AddItem(string, int): item does not fit container");
        }

        //Если такого типа предметов еще нет в контейнере, то добавляем его
        //TOTHINK: стоит перенести создание нового объекта для предмета в метод по поиску объекта предмета 
        //         (флаг на создание?)
        Item i = GetItemObject(objectType);
        if (i == null) {                       
            i = new Item(objectType, 0, ItemInfo.GetMaxStackSize(objectType));
            list.Add(i);
        }

        //добавляем предмет в контейнер и удаляем его из источника
        i.stackSize += amount;

        Debug.LogFormat("ItemAdded: {0}({1}), {2}", objectType, amount, this.ToString());

        //уведомляем заинтересованных об изменении контейнера
        Change(objectType, amount);
    }    

    //Функция для извлечения предмета из контейнера. 
    //Если указанного предмета нет в контейнере или не достаточное количество, 
    //то размер стака результата будет скоректирован
    //Никогда не возвращает null, но может вернуть предмет с stackSize == 0
    public Item RemoveItem(string objectType, int amount)
    {
        Item i = GetItemObject(objectType);

        if(i == null) {            
            i = new Item(objectType, 0, ItemInfo.GetMaxStackSize(objectType));
        }

        //Вычисляем сколько предметов мы можем отдать
        int count_to_remove = Mathf.Min(amount, i.stackSize);

        //Создаем объект для предмета который мы отдаем        
        Item result = new Item(objectType, 0, ItemInfo.GetMaxStackSize(objectType));

        //переклабываем предметы в результирующий контейнер
        result.stackSize += count_to_remove;
        i.stackSize -= count_to_remove;

        //Если забрали все, удаляем предмет из списка
        if(i.stackSize == 0) {
            list.Remove(i);
        }

        Debug.LogFormat("ItemRemoved: {0}({1}), {2}", objectType, amount, this.ToString());

        //Уведомляем об изменении состояния контейнера
        //TOTHINK: нужно ли здесь делать проверку на то, что мы извлекли > 0 предметов
        Change(objectType, -count_to_remove);

        return result;
    }    

    //Функция возвращает, сколько предметов указанного типа может быть размещено в контейнере
    public int CanAdd(string itemType)
    {
        //Если контейнер пуст, возвращаем максимальный размер стака для предмета
        if(list.Count == 0) {
            return ItemInfo.GetMaxStackSize(itemType);
        }
        
        //если предмет, такого типа не найден в контейнере, но контейнер не пуст, то возвращаем 0
        Item i = GetItemObject(itemType);
        if(i == null) {            
            return 0;
        }

        //если такой предмет есть, возвращаем сколько еще таких влезет
        return i.maxStackSize - i.stackSize;
    }    

    //Возвращает количество предметов указаного типа находящихся в контейнере
    public int HasItem(string itemType)
    {        
        Item i = GetItemObject(itemType);

        if(i == null) {
            return 0;
        }

        return i.stackSize;
    }
        
    public int GetVolime()
    {
        //TODO: возвращает максимальную доступную вместимость
        return 50;
    }

    public int GetEmptyVolime()
    {
        int volime = 0;
        for(int i = 0; i < list.Count; ++i) {
            volime += list[i].stackSize;
        }

        return Mathf.Max(GetVolime() - volime, 0);
    }

    //Удаляет контейнер с карты
    public void Remove()
    {
        if(cbContainerRemoved != null) {
            cbContainerRemoved(this);
        }
        
        //TODO: тут нужно открепить контейнер от тайла к которому он привязан

        //TODO: выбросить все предметы из контейнера на пол
    }

    public bool IsEmpty()
    {
        //TOTHINK: а если предметы в списке есть, но с пустым количеством предметов в стаке??

        return (list.Count == 0);
    }

    //-----------------   CALLBACK CALLERS   --------------------

    //Уведомляем об изменении состояния контейнера
    private void Change(string itemType, int amount)
    {
        cbContainerChanged?.Invoke(this);

        cbContainerItemChanged?.Invoke(this, itemType, amount);
    }

    //-----------------   TOOL FUNCTIONS   ----------------------
        
    /// <summary>
    /// Ищет в коллекции указанный тип предмета и возвращает его или null, если предмет не найден
    /// </summary>
    /// <param name="itemType">Тип предмета</param>
    /// <returns></returns>
    private Item GetItemObject(string itemType)
    {
        for(int i = 0; i < list.Count; ++i) {
            if(list[i].objectType == itemType) {
                return list[i];
            }
        }

        return null;
    }

    public override string ToString()
    {
        StringBuilder sb = new StringBuilder();

        sb.Append("Container");

        if(tile != null) {
            sb.AppendFormat(" [{0}:{1}]", tile.x, tile.y);
        }

        sb.Append(" {");

        for(int i = 0; i < list.Count; ++i) {
            sb.AppendFormat("{2}{0}({1})", list[i].objectType, list[i].stackSize, i == 0 ? "" : ",");
        }

        sb.Append("}");

        return sb.ToString();
    }
}
