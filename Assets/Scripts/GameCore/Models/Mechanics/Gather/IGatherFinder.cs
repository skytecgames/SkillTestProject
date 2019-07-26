using UnityEngine;
using System.Collections;

public interface IGatherFinder
{
    //TOFIX: GatherFinder должен учитывать при поиске тип операции сбора (сбор растений, добыча минералов, ... )

    //ищет ближайшую точку, на которой нужно собрать ресурсы
    WorkPoint GetClosestGatherPoint(Tile start);
}
