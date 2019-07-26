using UnityEngine;
using System.Collections;

public class GoalManager
{
    public void Init()
    {
        //Запускаем задачу на переноску предметов на склады
        Goal_HaulItemsToStockpile haul_goal = new Goal_HaulItemsToStockpile();
        haul_goal.Start();

        //Запускаем задачу на отдых
        Goal_Relax relax_goal = new Goal_Relax();
        relax_goal.Start();

        //Запускаем задачу на сбор ресурсов
        Goal_Harvest harvest_goal = new Goal_Harvest();
        harvest_goal.Start();
    }
}
