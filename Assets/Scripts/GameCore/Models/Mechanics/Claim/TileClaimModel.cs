using UnityEngine;
using System.Collections.Generic;

public class TileClaimModel
{
    private IDictionary<Tile, ClaimType> claims = new Dictionary<Tile, ClaimType>();

    //Зарезервировать тайл
    public void Claim(Tile tile, ClaimType type)
    {
        if(isFree(tile, type) == false) {
            Debug.LogErrorFormat("Tile [{0},{1}] already claimed for type {2}", tile.x, tile.y, type);
            return;
        }

        if(type == ClaimType.None) {
            Debug.LogErrorFormat("Can't claim tile [{0},{1}] for None", tile.x, tile.y);
            return;
        }

        // Устанавливаем флаги резервации
        if (claims.ContainsKey(tile) == false) {
            claims.Add(tile, type);
        } else {
            claims[tile] = claims[tile] | type;
        }

        Debug.LogFormat("Tile [{0},{1}] claimed for {2}", tile.x, tile.y, claims[tile]);
    }

    public void UnClaim(Tile tile, ClaimType type)
    {
        if(isFree(tile, type)) {
            Debug.LogErrorFormat("Tile [{0},{1}] not claimed for type {2}", tile.x, tile.y, type);
            return;
        }

        if (type == ClaimType.None) {
            Debug.LogErrorFormat("Can't unclaim tile [{0},{1}] for None", tile.x, tile.y);
            return;
        }

        if(claims.ContainsKey(tile) == false) {
            Debug.LogErrorFormat("Tile [{0},{1}] not found in claim map", tile.x, tile.y);
            return;
        }
                
        claims[tile] = claims[tile] & ~type;

        if (claims[tile] == ClaimType.None) {
            Debug.LogFormat("tile [{0},{1}] is free of claims", tile.x, tile.y);
            claims.Remove(tile);
        }
    }

    public bool isFree(Tile tile, ClaimType type)
    {
        if (type == ClaimType.None) return false;

        //Если нет в списке тайлов с резервацией значит свободен
        if (claims.ContainsKey(tile) == false) return true;

        return (claims[tile] & type) == 0;
    }
}
