using System;

public interface IChangeable<T>
{
    // Подписка/отписка на событие изменения объекта
    void RegisterOnChanged(Action<T> callback);
    void UnRegisterOnChanged(Action<T> callback);

    //Уведомление о событии изменения объекта
    void InvokeOnChanged();
}
