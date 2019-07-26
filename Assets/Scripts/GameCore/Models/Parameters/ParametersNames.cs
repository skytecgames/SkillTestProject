using UnityEngine;
using System.Collections;

public class ParameterName
{
    //TOTHINK: пока этот ключ нужен для сериализации параметров, но от него нужно избавиться
    public const int maxKey = 10;

    //Объект препятствует движению пока не открыт
    public const int openness = 1; 
    public const int is_opening = 2; 

    //Объект растет со временем
    public const int grow_stage = 3; 
    public const int grow_stage_max = 4;
    public const int grow_speed = 5;
    public const int grow_stage_time = 6;       //Время перехода на следующую стадию (абсолютное)
    public const int grow_is_active = 7;        //Объект растет в данный момент

    //прогресс строительства объекта
    public const int workProgress = 8;

    //Объект зависит от его окружения (отображение, функция)
    public const int links_to_neighbour = 9;

    //Объект является границей комнаты
    public const int room_enclosure = 10;

    //Объект можно собирать (указывает тип собираемого ресурса. Фактически метод, который используется для 
    //проверки возможности сбора ресурса)
    public const int gather_type = 11;

    public const int gather_finish_type = 12;
}
