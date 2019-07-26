using UnityEngine;
using System.Collections.Generic;

//Содержит все данные для растройки состояния точки работы
public class WorkPointInfo
{
    //Количество работы до завершения
    public float workAmount;

    //контейнер для материалов
    public List<KeyValuePair<string, int>> materials;

    //Обработка события - работа на точке начата
    //Тут имеется в виду не назначение агента, а сам факт размещения работы
    //Обработчик нужен например для строительства, где перед началом работы нужно разместить ее маркер на карте
    public System.Action<WorkPoint> OnStart;

    //Обработка события - работа на точке обновлена
    //Например кто-то изменил количество работы или добавлены новые материалы
    public System.Action<WorkPoint> OnChanged;

    //Обработка события - работа на точке завершена
    public System.Action<WorkPoint> OnComplete;

    //Обработка события - отмена работы
    public System.Action<WorkPoint> OnCancel;

    //Это должно вызываться при конструировании рабочей точки
    public void Init(WorkPoint workpoint)
    {
        //Настраиваем события для управления жизненым циклом точки работы
        workpoint.cbWorkStart += OnStart;
        workpoint.cbWorkChanged += OnChanged;
        workpoint.cbWorkComplete += OnComplete;
        workpoint.cbWorkCanceled += OnCancel;

        //Создаем контейнер под материалы
        workpoint.materials = new MaterialContainer();

        //Обновляем количество работы и материалы
        Reset(workpoint);

        //Запускаем работу
        workpoint.Start();
    }

    //Обновляет количество работы и материалы для точки работы
    public void Reset(WorkPoint workpoint)
    {
        workpoint.workAmount = workAmount;
        workpoint.materials.SetMaterials(materials);
    }
}
