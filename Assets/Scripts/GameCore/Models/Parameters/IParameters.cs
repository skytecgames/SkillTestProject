using System;

//Интерфейс для коллекции параметров объекта (персонажа, конструкта или тайла)
public interface IParameters : IChangeable<IParameters>, IRemoveable<IParameters>, ICloneable<IParameters>
{
    //TIPS: Методы интерфейса завязанны на контретные типы для оптимизации

    //Добавляет новый float параметр
    void SetFloat(int id, float value);

    //Извлекает float параметр по идентификатору
    float GetFloat(int id);

    //Увеличить float параметр по указаному идентификатору на delta
    void IncrementFloat(int id, float delta);

    //TOFIX: выборка параметров по тэгам    
}
