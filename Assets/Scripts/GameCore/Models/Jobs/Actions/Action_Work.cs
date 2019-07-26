using UnityEngine;
using System.Collections;

//Действие - работа на точке
public class Action_DoWork : Action
{
    private Agent agent;
    private WorkPoint workpoint;     

    public Action_DoWork(WorkPoint workpoint)
    {
        this.workpoint = workpoint;
        this.workpoint.cbWorkComplete += OnWorkComplete;
    }

    public override void SetAgent(Agent agent)
    {
        this.agent = agent;
    }

    public override void Update(float deltaTime)
    {
        //TOTHINK: нужна ли здесь проверка на то, что мы находимся на точке работы???

        //Если дошли до места работы - то работаем
        if (workpoint != null) {
            workpoint.DoWork(agent.character, deltaTime);
        }
    }

    public override void Cancel()
    {
        //для завершения этого действия нам ничего не нужно
    }

    private void OnWorkComplete(WorkPoint point)
    {
        if(workpoint != point) {
            Debug.LogError("OnWorkComplete with wrong workpoint!!!");
            return;
        }

        workpoint.cbWorkComplete -= OnWorkComplete;

        Complete();
    }

    private void Complete()
    {
        if(cbActionComplete != null) {
            cbActionComplete(this);
        }
    }
}
