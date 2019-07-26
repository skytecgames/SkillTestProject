using UnityEngine;
using System.Collections;

//Действие - ожидание на месте
public class Action_Wait : Action
{
    private float timer = 0;    

    public Action_Wait(float timer)
    {
        this.timer = timer;
    }    

    public override void Update(float deltaTime)
    {
        timer -= deltaTime;

        if(timer <= 0) {
            Complete();
        }
    }

    public override void Cancel()
    {
        //Ничего не делаем для отмены этого действия
    }

    private void Complete()
    {
        if (cbActionComplete != null) {
            cbActionComplete(this);
        }
    }
}
