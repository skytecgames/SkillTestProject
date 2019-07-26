using UnityEngine;
using System.Collections;

public static class InstalledObjectActions
{
    #region ------ DOOR ACTIONS ------

    public static void Door_UpdateAction(InstalledObject obj, float deltaTime)
    {
        //Debug.Log("Door_UpdateAction");

        if(obj.parameters.GetFloat(ParameterName.is_opening) >= 0.5f) {
            obj.parameters.IncrementFloat(ParameterName.openness, deltaTime * 4f);
            obj.parameters.SetFloat(ParameterName.openness, Mathf.Clamp01(obj.parameters.GetFloat(ParameterName.openness)));            
        } else {
            obj.parameters.IncrementFloat(ParameterName.openness, -deltaTime * 4f);
            obj.parameters.SetFloat(ParameterName.openness, Mathf.Clamp01(obj.parameters.GetFloat(ParameterName.openness)));            
        }

        obj.InvokeOnChanged();

        if (obj.parameters.GetFloat(ParameterName.is_opening) > 0)
        {
            obj.parameters.IncrementFloat(ParameterName.is_opening, -Mathf.Clamp(deltaTime, 0, 0.20f) * 2f);
        }
    }

    public static Enterability Door_IsEnterable(InstalledObject obj)
    {
        if (obj.parameters.GetFloat(ParameterName.openness) >= 1f)
        {
            return Enterability.Yes;
        }

        obj.parameters.SetFloat(ParameterName.is_opening, 1f);

        return Enterability.Soon;
    }

    #endregion

    #region ------ OXYGEN GENERATOR ACTIONS ------

    public static void OxygenGenerator_UpdateAction(InstalledObject obj, float deltaTime)
    {
        if (obj.tile.room.GetGasAmount("O2") < 0.2f) {
            //TODO: нужно учесть в расчетах размер комнаты
            obj.tile.room.ChangeGas("O2", 0.01f * deltaTime);
        }
    }

    #endregion    
}
