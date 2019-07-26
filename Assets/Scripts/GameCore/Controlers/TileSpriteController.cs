using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class TileSpriteController : MonoBehaviour
{      
    Dictionary<Tile, GameObject> tileGameObjectMap;    

    World world {
        get {
            return World.current;
        }
    }

	// Use this for initialization
	void Start ()
    {        
        tileGameObjectMap = new Dictionary<Tile, GameObject>();        

        for (int x = 0; x < world.width; ++x)
        {
            for (int y = 0; y < world.height; ++y)
            {
                GameObject tile_go = new GameObject();
                tile_go.name = string.Format("tile_{0}_{1}", x, y);

                tileGameObjectMap.Add(world[x, y], tile_go);

                SpriteRenderer tile_sr = tile_go.AddComponent<SpriteRenderer>();
                tile_sr.sprite = SpriteCollection.GetSprite(0);
                tile_sr.transform.position = new Vector3(x, y, 0);
                tile_sr.transform.SetParent(this.transform, true);
                tile_sr.sortingLayerName = "Tile";

                OnTileChanged(world[x, y]);
            }
        }

        world.cbTileChanged += OnTileChanged;        
    }   

    /*
    void DestroyAllTileGameObject()
    {
        while(tileGameObjectMap.Count > 0)
        {
            Tile t = tileGameObjectMap.Keys.First();
            GameObject t_go = tileGameObjectMap[t];

            tileGameObjectMap.Remove(t);

            t.cbTileTypeChanged -= OnTileChanged;
            GameObject.Destroy(t_go);
        }
    }
    */

    void OnTileChanged(Tile tile)
    {
        if(tileGameObjectMap.ContainsKey(tile) == false) {
            Debug.LogError("tileGameObjectMap dont contain data for requested tile");
            return;
        }

        GameObject obj = tileGameObjectMap[tile];

        if(obj == null)
        {
            Debug.LogError("tileGameObjectMap dont contain object for requested tile");
            return;
        }

        SpriteRenderer tile_sr = obj.GetComponent<SpriteRenderer>();
        tile_sr.sprite = SpriteCollection.GetSprite((int)tile.Type);        
    }        
}
