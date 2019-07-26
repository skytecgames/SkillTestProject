using System;

public interface IRemoveable<T>
{
    //Подписка/отписка на события удаления объекта
    void RegisterOnRemoved(Action<T> callback);
    void UnRegisterOnRemoved(Action<T> callback);
}
