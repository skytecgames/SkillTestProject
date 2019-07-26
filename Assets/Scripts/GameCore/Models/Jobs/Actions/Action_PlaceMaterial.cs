using UnityEngine;
using System.Collections;

//Действие - добавление материала в точку работы из инвентаря персонажа
public class Action_PlaceMaterial : Action
{
    //персонаж выполняющий действие
    public Agent agent;

    //точка для работы в которую вкидываются материалы
    public WorkPoint workpoint;

    //тип предмета
    public string itemType;

    //количество предметов
    public int amount;

    public Action_PlaceMaterial(WorkPoint workpoint, string itemType, int amount)
    {
        Debug.LogFormat("Action_PlaceMaterial, {0}({1}) to [{2}:{3}]", itemType, amount, workpoint.tile.x, workpoint.tile.y);

        this.workpoint = workpoint;
        this.itemType = itemType;
        this.amount = amount;
    }

    public override void SetAgent(Agent agent)
    {
        this.agent = agent;
    }

    public override void Update(float deltaTime)
    {
        if (workpoint == null) {
            Debug.LogError("place material action on null workpoint");
            return;
        }

        if (agent == null || agent.character == null) {
            Debug.LogError("place material action try to update without agent to do it");
            return;
        }

        if (workpoint.tile != agent.character.currTile) {
            //TOFIX: добавить возможность брать предметы с соседнего тайла

            Debug.LogError("character try to place material from distance");
            return;
        }

        //transfer items from character to workpoint
        int transfered = ItemManagerA.TransferItem(agent.character.itemContainer, workpoint, itemType, amount);

        if(transfered < amount) {
            Debug.LogErrorFormat("Action_PlaceMaterial: transfered {0} / {1} items", transfered, amount);
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
