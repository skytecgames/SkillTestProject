using UnityEngine;
using System.Collections.Generic;

//Набор статических методов для проверки возможности сбора ресурсов с указанного объекта
public class GatherTypeActions
{
    //Идентификаторы типов функций проверки возможности собирать ресурсы
    public const int GatherType_FullyGrown = 1;

    //Идентификаторы типов функций завершения процесса сбора ресурса
    public const int GatherFinishType_ResetStage = 1;

    //Коллекция соответствий идентификатора функции и реализации
    private static IDictionary<int, System.Func<IParameters, bool>> actions = new Dictionary<int, System.Func<IParameters, bool>>()
    {
        { GatherType_FullyGrown, GatherFullyGrown},
    };
    //Коллекция соответствий идентификатора функции и реализации
    private static IDictionary<int, System.Action<IParameters>> finishers = new Dictionary<int, System.Action<IParameters>>()
    {
        { GatherFinishType_ResetStage, GatherFinishResetStage },
    };

    //Проверка объекта на возможность собрать с него ресурсы
    public static bool IsReady(IParameters obj)
    {
        System.Func<IParameters, bool> func = GetGatherAction((int)obj.GetFloat(ParameterName.gather_type));
        if(func != null) {
            return func(obj);
        }

        return false;
    }

    //Завершение сбора ресурсов с объекта
    public static void Finish(IParameters obj)
    {
        System.Action<IParameters> func = GetGatherFinishAction((int)obj.GetFloat(ParameterName.gather_finish_type));
        if(func != null) {
            func(obj);
        }
    }

    //выдает функцию проверки возможности сбора ресурса по идентификатору типа
    private static System.Func<IParameters, bool> GetGatherAction(int type)
    {
        if(actions.ContainsKey(type)) {
            return actions[type];
        }

        return null;
    }

    private static System.Action<IParameters> GetGatherFinishAction(int type)
    {
        if(finishers.ContainsKey(type)) {
            return finishers[type];
        }

        return null;
    }

    //Проверка: можно собирать если на последней стадии роста
    private static bool GatherFullyGrown(IParameters parameters)
    {
        int stage = (int)parameters.GetFloat(ParameterName.grow_stage);
        int stage_max = (int)parameters.GetFloat(ParameterName.grow_stage_max);

        //Если максимальная стадия роста 0, то объект не растет
        if (stage_max == 0) {
            Debug.LogError("GatherFullyGrown is for growable objects only");
            return false;
        }

        return stage == stage_max;
    }

    //Заверщение: сбросить стадию роста на 1
    private static void GatherFinishResetStage(IParameters parameters)
    {
        //меняем стадию на первую
        parameters.SetFloat(ParameterName.grow_stage, 1);

        //уведомляем всех о смене параметров объекта
        parameters.InvokeOnChanged();
    }
}
