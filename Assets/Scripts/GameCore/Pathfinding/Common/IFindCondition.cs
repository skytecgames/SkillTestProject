using UnityEngine;
using System.Collections;

public interface IFindCondition<T>
{
    bool Check(T node);
}
