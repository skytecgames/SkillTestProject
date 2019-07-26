using UnityEngine;
using Pathfinding;

//Действие - перемещение в указанную точку
public class Action_MoveToPoint : Action
{
    //Персонаж, который движется этим действием
    public Agent agent;
    //Тайл в который мы движемся
    private Tile destTile;
    //Путь, по которому мы движемся
    private IPath<Tile> path;    

    //Distance to next tile
    private float distToTravel = 0;

    public Action_MoveToPoint(Tile destTile)
    {
        this.destTile = destTile;
    }

    public override void SetAgent(Agent agent)
    {
        if (agent == null) {
            Debug.LogError("agent is null");
            return;
        }

        if (agent.character == null) {
            Debug.LogError("character is null");
            return;
        }

        this.agent = agent;
    }

    public override void Update(float deltaTime)
    {        
        //если мы в конечной точке, то ничего не обновляем
        if (agent.character.currTile == destTile) {
            Complete();
            return;
        }

        //Если достигли следующей точки пути, ищем след точку
        if (agent.character.nextTile == agent.character.currTile) {
            //если путь не построен, строим его
            if (path == null) {
                
                IPathfinder<Tile> pathfinder = PathfindingManager.GetPathfinder(agent.character.currTile, destTile);
                pathfinder.Find();
                path = pathfinder.GetResult();

                if (path != null && path.Length() > 0) {
                    agent.character.nextTile = path.GetNext();
                }
            }

            //если путь к точке не найден, вернем задачу в список задач
            if (path == null || path.Length() == 0) {
                Debug.Log("Character.Update_DoMovement error: can't find path");

                //путь не может быть найден, останавливаем работу                
                Stop();

                path = null;
                return;
            }

            //если есть путь, берем следующий его отрезок
            agent.character.nextTile = path.GetNext();

            //вычисляем расстояние между текущим и следующим тайлом
            distToTravel = Mathf.Sqrt(Mathf.Pow(agent.character.currTile.x - agent.character.nextTile.x, 2)
            + Mathf.Pow(agent.character.currTile.y - agent.character.nextTile.y, 2));
        }        

        //по пути встретили ранее не учтенное препятствие, перестроим путь
        if (agent.character.nextTile.isEnterable == Enterability.Never) {
            Debug.LogError("character try to enter unwalkable tile. Recalculating path ...");
            agent.character.nextTile = agent.character.currTile;
            path = null;
            return;
        }

        //по пути встретился тайл с временной непроходимостью (дверь например)
        //запрашивая у такого объекта isEnterable мы открываем его
        if (agent.character.nextTile.isEnterable == Enterability.Soon) {
            return;
        }

        //вычисляем на сколько мы продвинулись за полученное на движение время
        float distThisFrame = agent.character.speed / agent.character.nextTile.movementCost * deltaTime;
        float percThisFrame = distThisFrame / distToTravel;

        agent.character.movePercentage += percThisFrame;

        if (agent.character.movePercentage >= 1f)
        {
            agent.character.currTile = agent.character.nextTile;
            agent.character.movePercentage = 0;

            //TOFIX: use unspended move?
        }
    }

    public override void Cancel()
    {
        //TODO: для отмены этого действия мы должны дойти до следующей точки пути
    }

    private void Complete()
    {
        if(cbActionComplete != null) {
            cbActionComplete(this);
        }
    }

    private void Stop()
    {
        if(cbActionStop != null) {
            cbActionStop(this);
        }
    }
}
