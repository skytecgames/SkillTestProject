using UnityEngine;
using System.Collections;

namespace Pathfinding
{
    public interface IMoveCost
    {
        float movementCost { get; }
    }
}
