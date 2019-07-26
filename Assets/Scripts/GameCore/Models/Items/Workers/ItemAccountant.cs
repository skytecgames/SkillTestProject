using UnityEngine;
using System.Collections.Generic;
using Pathfinding.ZStar;

//Класс для учета количества предметов в игре
public class ItemAccountant
{
    //Список всех непустых контейнеров в игре
    private List<ItemContainer> itemContainers;

    //Количество разных типов предметов в игре
    private Dictionary<string, int> itemCount;

    //Класс для учета предметов на складах
    private StockpileOverseer stockpiles;

    public ItemAccountant(World world, StockpileOverseer stockpiles)
    {
        this.stockpiles = stockpiles;

        //Инициализируем данные
        itemContainers = new List<ItemContainer>();
        itemCount = new Dictionary<string, int>();        

        //Подписываемся на создание контейнеров
        world.cbItemContainerCreated += Subscribe;
    }    

    //Перечисление всех непустых контейнеров в игре в которых есть указанный тип предмета
    public IEnumerator<ItemContainer> ContainersWithItem(string itemType)
    {
        for(int i = 0; i < itemContainers.Count; ++i) {
            if (itemContainers[i].HasItem(itemType) > 0) yield return itemContainers[i];
        }
    }

    //Перечисление всех контейнеров в игре на которых есть готовые к переноске на склад предметы
    public IEnumerator<ItemContainer> ContainersWithHaulable()
    {
        //TOFIX: сделать отдельный список по этому критерию и просто возвращать предметы из него

        for (int i = 0; i < itemContainers.Count; ++i) {            
            if (stockpiles.NeedHauling(itemContainers[i]) != null) yield return itemContainers[i];
        }        
    }

    //Количество предметов в игре
    public int GetItemCount(string itemType)
    {
        if (itemCount.ContainsKey(itemType)) return itemCount[itemType];

        return 0;
    }    

    private void Subscribe(ItemContainer cont)
    {
        //Если это подвижный контейнер, то мы его игнорируем
        if(cont.tile == null) return;

        //Подписываемся на изменение состояния предметов в контейнере
        cont.cbContainerItemChanged += OnContainerChanged;
    }

    private void UnSubscribe(ItemContainer cont)
    {
        //Если это подвижный контейнер, то мы его игнорируем
        if (cont.tile == null) return;

        cont.cbContainerItemChanged -= OnContainerChanged;
    }

    private void OnContainerChanged(ItemContainer cont, string itemType, int amount)
    {
        //REMEMBER: мы не проверяем является ли этот контейнер мобильным, так как на такие мы не должны быть подписаны

        //Если еще нет такого типа предметов в учете, создадим для него запись
        if (itemCount.ContainsKey(itemType) == false) itemCount.Add(itemType, 0);

        //Применяем изменение количества предметов
        itemCount[itemType] += amount;

        //Количество предметов не может быть отрицательным
        if(itemCount[itemType] < 0) {
            Debug.LogErrorFormat("ItemCount: [{0} == {1}] is negative number", itemType, itemCount[itemType]);
        }

        //Если контейнер пустой, но содержится в списке непустых контейнеров, то удаляем его оттуда
        if(cont.IsEmpty() && itemContainers.Contains(cont)) {
            itemContainers.Remove(cont);
        }

        //Если контейнер не пустой и его нет в списке, добавляем его туда
        if(cont.IsEmpty() == false && itemContainers.Contains(cont) == false) {
            itemContainers.Add(cont);
        }
    }    
}
