using UnityEngine;
using System.Collections;
using System;

//Тип резервации (под какой тип задачи зарезервирован тайл)
[Flags]
public enum ClaimType
{
    None = 0,
    Gather = 1,
}

// Менеджер механики резервирования тайлов для выполнения задач
public class TileClaimManager
{
    private static TileClaimModel model;

    //Инициализация менеджера
    public static void Init()
    {
        model = new TileClaimModel();
    }

    //Зарезервировать тайл
    public static void Claim(Tile tile, ClaimType type)
    {
        model.Claim(tile, type);
    }

    //Снять резервацию с тайла
    public static void UnClaim(Tile tile, ClaimType type)
    {
        model.UnClaim(tile, type);
    }

    //Проверить наличие резервании для тайла
    public static bool IsFree(Tile tile, ClaimType type)
    {
        return model.isFree(tile, type);
    }
}
