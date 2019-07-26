using UnityEngine;
using System.Collections;

//Действие - положить предмет в указанный контейнер
public class Action_PlaceItem : Action
{
    //Персонаж, выполняющий действие
    public Agent agent;

    //Контейнер в который планируется размещение предмета
    private ItemContainer container;

    //Тип предмета, коотрый нужно разместить
    private string itemType;

    //Количество предметов, которые нужно разместить
    private int amount;    

    public Action_PlaceItem(ItemContainer container, string itemType, int amount)
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
        if (container == null) {
            Debug.LogError("action try pick item from null container");
            return;
        }

        if (agent == null || agent.character == null) {
            Debug.LogError("action try to update without agent to do it");
            return;
        }

        if (container.tile != agent.character.currTile) {

            //TOFIX: добавить возможность брать предметы с соседнего тайла

            Debug.LogError("character try to pick item from distance");
            return;
        }

        //TOFIX: добавить учет времени на размещение предмета        

        //transfer items from character to container
        int transferred = ItemManagerA.TransferItem(agent.character.itemContainer, container, itemType, amount);

        if(transferred < amount) {
            //TOFIX: return message when job will be calculating exact item amount
            //Debug.LogErrorFormat("Action_PlaceItem: place {0} / {1} items", transferred, amount);
        }

        Complete();
    }

    public override void Cancel()
    {
        //ничего не делаем для отмены этого действия

        //TOTHINK: возможно имеет смысл выбросить предмет или вернуть его на место (а то получится что персонаж носит предмет
        //         необходимый для завершения работы и таким образом блокирует достижение цели)
    }

    private void Complete()
    {
        if (cbActionComplete != null) {
            cbActionComplete(this);
        }
    }
}
