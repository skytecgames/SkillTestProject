using UnityEngine;
using System.Collections.Generic;

//Класс подволяющий выполнять персонажем простую работу, состоящую из последовательности действий (Action)
//Экземпляры этого класса создаются с помощью JobPlaner
//Для каждого экземпляра Job нужен свой агент для выполнения
public class JobNew
{
    //персонаж выполняющий работу
    public Agent agent;

    //тип задачи (Наследуется из JobInfo)
    public JobType type;

    //список действий для выполнния
    public List<Action> action_list;

    //индекс текущего выполняемого действия
    private  int currActionIndex = 0;

    //События для управления жизненым циклом работы
    public System.Action<JobNew> cbJobComplete;
    public System.Action<JobNew> cbJobStop;

    public void Init()
    {
        if(action_list == null) {
            Debug.LogError("Init job with null action list");
            return;
        }

        //Назначаем обработчики событий всем действиям
        for(int i = 0; i < action_list.Count; ++i) {
            action_list[i].cbActionComplete = OnActionComplete;
            action_list[i].cbActionStop = OnActionStop;

            action_list[i].SetAgent(agent);
        }
    }

    public void Update(float deltaTime)
    {
        if(action_list == null) {
            Debug.LogError("job is updating with action list == null");
            return;
        }

        if(agent == null) {
            Debug.LogError("job is update with agent == null");
            return;
        }

        if(currActionIndex >= action_list.Count || currActionIndex < 0) {
            Debug.Log("job next action index is out of bounds. Job is complete???");
            return;
        }

        action_list[currActionIndex].Update(deltaTime);
    }

    //отмена задания
    public void Cancel()
    {        
        //TOTHINK: возможно тут должен быть другой метод, невозможность выполнить и отмена разные вещи

        Stop();
    }

    //обработчик события о завершении действия
    private void OnActionComplete(Action action)
    {
        if (currActionIndex >= action_list.Count || currActionIndex < 0) {
            Debug.Log("job next action index is out of bounds");
            return;
        }

        if(action_list[currActionIndex] != action) {
            Debug.Log("job action complete, but it is not current action");
            return;
        }

        //следующее действие
        currActionIndex++;

        if(currActionIndex >= action_list.Count) {
            Complete();
        }
    }

    //обработчик события - прерывание работы по причине невозможности выполнения одного из действий
    private void OnActionStop(Action action)
    {        
        Stop();
    }

    private void Complete()
    {
        if(cbJobComplete != null) {
            cbJobComplete(this);
        }
    }  
    
    private void Stop()
    {
        if(cbJobStop != null) {
            cbJobStop(this);
        }
    }
}
