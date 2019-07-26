using UnityEngine;
using System.Collections.Generic;

public interface IWorkQueue
{
    //Текущий производимый элемент
    WorkPointInfo Current();

    //Переключиться на следующий элемент
    WorkPointInfo Next();

    //Предметы производимые текущим рецептом
    List<KeyValuePair<string, int>> Product();
}
