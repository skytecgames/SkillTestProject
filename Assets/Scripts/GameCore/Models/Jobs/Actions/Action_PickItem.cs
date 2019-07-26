using UnityEngine;
using System.Collections;

//Действие - взять предмет из указанного контейнера
public class Action_PickItem : Action
{
    //персонаж для совершения действия
    public Agent agent;

    //контейнер из которого мы должны взять предмет
    public ItemContainer container;

    //тип предмет, который нужно подобрать
    public string itemType;
    
    //количество предметов, которое нужно подобрать
    public int amount;    

    public Action_PickItem(ItemContainer container, string itemType, int amount)
    {
        this.container = container;
        this.itemType = itemType;
        this.amount = amount;
    }

    public override void SetAgent(Agent agent)
    {
        this.agent = agent;
    }

    public override void Update(float deltaTime)
    {
        if(container == null) {
            Debug.LogError("action try pick item from null container");
            return;
        }

        if(agent == null || agent.character == null) {
            Debug.LogError("action try to update without agent to do it");
            return;
        }

        if(container.tile != agent.character.currTile) {
            
            //TOFIX: добавить возможность брать предметы с соседнего тайла

            Debug.LogError("character try to pick item from distance");
            return;
        }

        //вытаскиваем предмет из контейнера
        int transfered = ItemManagerA.TransferItem(container, agent.character.itemContainer, itemType, amount);

        if(transfered < amount) {
            Debug.LogErrorFormat("Action_PickItem: pick {0} / {1} items (space:{2})", transfered, amount, 
                agent.character.itemContainer.GetEmptyVolime());
        }

        //TOFIX: добавить учет времени на подбор предмета

        //задача завершена
        Complete();
    }

    public override void Cancel()
    {
        //ничего не нужно делать для отмены этого действия
    }

    private void Complete()
    {
        if (cbActionComplete != null) {
            cbActionComplete(this);
        }
    }
}
