using UnityEngine;
using System.Collections.Generic;

//Базовый класс для элементарного действия
public class Action
{    
    // Установить исполнителя для выполнения действия    
    public virtual void SetAgent(Agent agent) { }

    //Производит действия для которого предназначен этот объект
    public virtual void Update(float deltaTime) { }

    //Прекращает действие
    public virtual void Cancel() { }

    //Уведомление о завершении действия
    public System.Action<Action> cbActionComplete;
    public System.Action<Action> cbActionStop;
}
