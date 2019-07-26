using UnityEngine;
using System.Collections.Generic;

//Составная задача. Разные части этой задачи могут выполнять разные агенты
public class Goal
{
    //Старт задачи (инициализация)
    public virtual void Start() { }

    //Остановка задачи (удаление)
    public virtual void Stop() { }

    //Пауза задачи
    public virtual void Pause(bool state) { }    
}
