using UnityEngine;
using System.Collections.Generic;

//Класс для управления перемещением предметов на склады
//Предполагается что у этого класса будет только один экзкмпляр
//новые склады просто добавляются в него в качестве областей, куда перемещаются предметы
public class Goal_HaulItemsToStockpile : Goal
{
    //Не имеет смысла для каждого предмета на карте генерировать свою задачу на переноску
    //Лучше держать в JobAgent только одну задачу, которую любой агент может взять (и она никогда не исчезает из списка
    //пока есть что переносить)

    //Эта задача будет сформулирована: перенеси ближайший к тебе предмет на склад        

    public JobInfo_HaulItemToStockpile haul_job;

    public override void Start()
    {
        //Эта задача должна стартовать только 1 раз
        if(haul_job != null) {
            Debug.LogError("Goal_HaulItemsToStockpile: is started more then one times");
            return;
        }

        //Создаем задачку на переноску
        haul_job = new JobInfo_HaulItemToStockpile();
        //TODO: установить правильный тип задачи для переноски предметов (пока приоритет стоит как у Construction)
        haul_job.type = JobType.Haul;

        //регистрируем задачу в JobAgent
        JobAgent.GetInstance().AddJob(haul_job);

        //больше ничего не делаем
    }    
}
