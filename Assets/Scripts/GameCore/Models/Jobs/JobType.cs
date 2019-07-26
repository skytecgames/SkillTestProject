using UnityEngine;
using System.Collections;

public enum JobType
{
    None,           //неопределенный тип задачи
    Haul,           //переноска предметов
    Construction,   //размещение предметов (строительство)
    Crafting,       //Производство предметов на станке    
    Need,           //Задача восполнение нужд персонажа
    Gather,         //Сбор ресурсов
    Wait            //Задача на случай, если персонажу нечего делать
}
