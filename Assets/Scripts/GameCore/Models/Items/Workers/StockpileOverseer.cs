using UnityEngine;
using System.Collections.Generic;

public class StockpileOverseer
{
    //Идентификатор свободного места под любой предмет
    private const string any = "any_item";

    //Список всех контейнеров с складами
    private List<ItemContainer> stockpiles;

    //Количество свободного места под различные предметы на складах
    private Dictionary<string, int> spaceCount;

    //Конструктор
    public StockpileOverseer()
    {
        stockpiles = new List<ItemContainer>();
        spaceCount = new Dictionary<string, int>();

        //Добавляем ключ для любых предметов
        spaceCount.Add(any, 0);
    }

    //Обработка события - изменения контейнера с предметами
    private void OnStockpileChanged(ItemContainer container, string itemType, int amount)
    {
        //TOFIX: оптимизация перерасчета количества доступного на складах места

        //Пересчитываем место на всем складе
        RecalculateSpace();
    }

    //Обработка события - удаление склада
    private void OnStockpileRemoved(ItemContainer container)
    {
        UnRegister(container);
    }    

    //Пересчитываем доступное на складе место
    private void RecalculateSpace()
    {
        //TOFIX: очистка списка работает неэффективно!!!

        //Очищаем текущие данные учета        
        IEnumerator<string> it = spaceCount.Keys.GetEnumerator();
        List<string> keys = new List<string>();
        while (it.MoveNext()) {         
            keys.Add(it.Current);
        }

        for(int i = 0; i < keys.Count; ++i) {
            spaceCount[keys[i]] = 0;
        }        

        //Перебираем все контейнеры склада
        for (int i = 0; i < stockpiles.Count; ++i) {
            ItemContainer cont = stockpiles[i];

            if(cont.IsEmpty()) {                
                spaceCount[any] += cont.GetVolime();
            } else {
                string itemType = cont.GetTopItem().objectType;
                if (spaceCount.ContainsKey(itemType) == false) spaceCount.Add(itemType, 0);
                spaceCount[itemType] += cont.GetEmptyVolime();
            }
        }
        
    }

    //Добавить контйнер в список складов
    public void Register(ItemContainer container)
    {
        if (stockpiles.Contains(container) == false) {

            //Подписываемся на события контейнера
            container.cbContainerItemChanged += OnStockpileChanged;
            container.cbContainerRemoved += OnStockpileRemoved;
            
            //Добавляем в общий список
            stockpiles.Add(container);

            //пересчитываем доступное на складе место
            RecalculateSpace();
        }
    }

    //Удалить контейнер из списка складов
    public void UnRegister(ItemContainer container)
    {
        //Проверяем, есть ли у нас такой контейнер
        if (stockpiles.Contains(container) == false) {
            Debug.LogError("StockpileManager: RemoveStockpile for non stockpile container");
            return;
        }

        //Отписываемся о событий
        container.cbContainerItemChanged -= OnStockpileChanged;
        container.cbContainerRemoved -= OnStockpileRemoved;

        //Удаляем из списка
        stockpiles.Remove(container);

        //пересчитываем доступное на складе место
        RecalculateSpace();
    }

    //Проверяет наличие места на складах под указанный тип предмета
    public int CanBeStockpiled(string itemType)
    {
        int amount = 0;        

        //Если есть место под конкретно такой тип предмета, добавляем его
        if(spaceCount.ContainsKey(itemType)) {
            amount += spaceCount[itemType];
        }

        //Добавляем место для хранения любых предметов
        if (spaceCount.ContainsKey(any)) {
            amount += spaceCount[any];
        }

        return amount;
    }    

    //Возвращает, есть ли в указанном контейнере предметы для переноски
    public string NeedHauling(ItemContainer cont)
    {       
        if(cont.tile == null) {
            Debug.LogError("NeedHauling: check mobile container");
            return null;
        }

        if(cont.IsEmpty()) {
            Debug.LogError("NeedHauling: check empty container");
            return null;
        }

        //Если контейнер не на складе, то возвращаем первый предмет из него
        if(cont.tile.installedObject == null || cont.tile.installedObject.objectType != "Stockpile") {
            return cont.GetTopItem().objectType;
        }

        //Если контейнер со склада, но содержит не подходящие предметы то вернем один из них
        if(cont.tile.installedObject != null && cont.tile.installedObject.objectType == "Stockpile" 
            && cont.GetItemTypesCount() > 1) {
            List<string> i_types = cont.GetItemTypes();
            string i_top = cont.GetTopItem().objectType;

            for(int i = 0; i < i_types.Count; ++i) {
                if (i_types[i] != i_top) return i_types[i];
            }
        }

        return null;
    }

    //Перечисление контейнеров с складами на которых можно хранить указанный тип предметов
    public IEnumerator<ItemContainer> ContainersWithStockpile(string itemType)
    {
        for (int i = 0; i < stockpiles.Count; ++i) {

            //если пустой, возвращаем его
            if (stockpiles[i].IsEmpty()) yield return stockpiles[i];

            //Если нет места под указанный тип предметов, то пропускаем контейнер
            if (stockpiles[i].CanAdd(itemType) == 0) continue;

            //Если склад содержит именно такие предметы, по возвращаем его
            if (stockpiles[i].GetTopItem().objectType == itemType) yield return stockpiles[i];
        }
    }

    //Перечисление контейнеров со складами на которых уже хранится указанный предмет и есть еще свободное место
    public IEnumerator<ItemContainer> ContainersWithStockOfItems(string itemType)
    {
        for (int i = 0; i < stockpiles.Count; ++i) {

            //если пустой, возвращаем его
            if (stockpiles[i].IsEmpty()) continue;

            //Если нет места под указанный тип предметов, то пропускаем контейнер
            if (stockpiles[i].CanAdd(itemType) == 0) continue;

            //Если склад содержит именно такие предметы, по возвращаем его
            if (stockpiles[i].GetTopItem().objectType == itemType) yield return stockpiles[i];
        }
    }

    //Перечисление контейнеров со пустыми складами (на которых разрешен указанный тип предметов)
    public IEnumerator<ItemContainer> ContainersWithEmptyStock(string itemType)
    {
        for (int i = 0; i < stockpiles.Count; ++i) {

            //если пустой, возвращаем его
            if (stockpiles[i].IsEmpty()) yield return stockpiles[i];            
        }
    }
}
