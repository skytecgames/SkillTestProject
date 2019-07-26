using UnityEngine;
using System.Collections.Generic;

public class FindCondition_Spawn : IFindCondition<Tile>
{
    //Тип и количество предметов, которые нужно разместить
    private string itemType;
    private int amount;
    private Tile startTile;
    private int MaxDistance = 5;

    //Расстояние до ближайшего подходящего тайла (если тайкой есть)
    private int distance = 0;
    private Queue<Tile> emptyList;

    //Найденный тайл
    private List<Tile> result;

    public FindCondition_Spawn()
    {
        emptyList = new Queue<Tile>();
        result = new List<Tile>();
    }

    //Инициадизируем условие поиска
    public void Init(string itemType, int amount, Tile startTile)
    {
        this.itemType = itemType;
        this.amount = amount;
        this.startTile = startTile;

        distance = 0;
        emptyList.Clear();
        result.Clear();
    }

    public bool Check(Tile tile)
    {
        //вычисляем "дистанцию"
        int dist = Mathf.Max(Mathf.Abs(tile.x - startTile.x), Mathf.Abs(tile.y - startTile.y));

        //Поиск ушел за максимальную дистанцию, прекращаем его
        if (dist > MaxDistance) return true;

        //Если нашли тайл на котором уже лежат предметы нужного типа
        if (tile.itemContainer != null && tile.itemContainer.HasItem(itemType) > 0) {

            //вычисляем, сколько предметов мы можем разместить в тайле
            int canadd = tile.itemContainer.CanAdd(itemType);

            //Если 0, то пропускаем тайл
            if (canadd == 0) return false;

            //Если на размещение всех предметов не хватит места, то добавляем тайл к результатам поиска и продолжаем поиск
            if(amount > canadd) {
                result.Add(tile);
                amount -= canadd;
                return false;
            }

            //Если нашли достаточно тайлов для размещения результата, то прекращаем поиск
            result.Add(tile);
            amount = 0;
            return true;
        }        

        //Если дистанция поиска увеличилась, то проверяем нет ли у нас достаточно свободных тайлов для спавна предметов
        if(distance < dist) {
            while(emptyList.Count > 0) {
                Tile t = emptyList.Dequeue();

                //TODO: нужны функции для проверки canAdd без инициализации контейнера
                int canadd = 50;

                //Добавляем тайл в результаты поиска
                result.Add(t);
                amount -= canadd;                

                if (amount <= 0) {
                    amount = 0;
                    return true;
                } 
            }
        }

        //если найден пустой тайл, добавим его в список пустых тайлов
        if(tile.itemContainer == null || tile.itemContainer.IsEmpty()) {

            //TODO: проверим отсутствие клэймов на размещение предметов

            //Добавим найденный тайл в список пустых тайлов и скоректируем расстояние
            emptyList.Enqueue(tile);
            distance = dist;
        }

        return false;
    }

    //Результат поиска
    public List<Tile> GetResult()
    {
        return result;
    }
}
