using Pathfinding;
using Pathfinding.ZStar;

public class PathfindingManager
{   
    public static void Init(World world)
    {
        PathfindingManagerZ.Init(world);
    }

    //Получить готовую к использованию копию объекта для поиска пути из A в B
    public static IPathfinder<Tile> GetPathfinder(Tile A, Tile B)
    {
        return PathfindingManagerZ.GetPathfinder(A, B);
    }   

    //Получить готовую к использованию копию поисковика тайла с условием завершения
    public static IPathfinder<Tile> GetConditionFinder(Tile A, IFindCondition<Tile> condition)
    {
        return PathfindingManagerZ.GetConditionFinder(A, condition);
    }
}
