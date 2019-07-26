using System;
using System.Collections.Generic;
using UnityEngine;

//Класс для хранения параметров сущностей игры (персонажей, конструктов, тайлов)
public class Parameters : IParameters
{
    //TOTHINK: мы используем конструктор конкретного класса вместо Inject при создании коллеций.
    //         Инъекция может помочь нам быстро сменить все коллеции. Но нужно ли нам менять их везде?

    //коллекция параметров
    private IDictionary<int, float> collect_float;

    //Событие: параметры объекта изменились
    private Action<IParameters> cbOnParametersChanged;

    //Событие: объект удален
    private Action<IParameters> cbOnParametersRemoved;

    public Parameters()
    {              
        collect_float = new Dictionary<int, float>();
    }

    //подписка на событие на изменение параметров
    public void RegisterOnChanged(Action<IParameters> callback)
    {
        cbOnParametersChanged += callback;
    }

    //отписка от события изменения параметров
    public void UnRegisterOnChanged(Action<IParameters> callback)
    {
        cbOnParametersChanged -= callback;
    }

    //Вызов события уведомления об изменении параметров
    public void InvokeOnChanged()
    {        
        if(cbOnParametersChanged != null) {
            cbOnParametersChanged(this);
        }
    }

    //Подписка на событие удаление объекта
    public void RegisterOnRemoved(Action<IParameters> callback)
    {
        cbOnParametersRemoved += callback;
    }

    //Отписка от события удаления объекта
    public void UnRegisterOnRemoved(Action<IParameters> callback)
    {
        cbOnParametersRemoved -= callback;
    }

    //Создает или меняет значение с указанным идентификатором и тэгами
    public void SetFloat(int id, float value)
    {
        collect_float[id] = value;        
    }

    //Увеличивает значение на указанный delta
    public void IncrementFloat(int id, float delta)
    {
        collect_float[id] += delta;
    }

    //Получает значение по указанному идентификатору
    public float GetFloat(int id)
    {
        if(collect_float.ContainsKey(id)) {
            return collect_float[id];
        }

        return 0;        
    }

    //Клонирование
    public IParameters Clone()
    {
        Parameters clone = new Parameters();

        //Копируем коллекции параметров
        IEnumerator<KeyValuePair<int, float>> e = collect_float.GetEnumerator();
        while(e.MoveNext()) {
            clone.collect_float.Add(e.Current);
        }

        return clone;
    }
}
