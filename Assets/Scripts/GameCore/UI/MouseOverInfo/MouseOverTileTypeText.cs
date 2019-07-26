using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class MouseOverTileTypeText : MonoBehaviour
{
    MouseController mouseController;
    Text text;

    // Use this for initialization
    void Start()
    {
        text = GetComponent<Text>();
        if(text == null)
        {
            Debug.LogError("cant find text component");
            this.enabled = false;
        }

        mouseController = FindObjectOfType<MouseController>();
        if(mouseController == null)
        {
            Debug.LogError("cant find mouse controller");
            this.enabled = false;
        }
    }

    // Update is called once per frame
    void Update() {
        Tile t = mouseController.GetTileUnderMouse();

        text.text = string.Format("TileType: {0} [{1}:{2}]", 
            t == null ? "Null" : t.Type.ToString(), 
            t == null ? "X" : t.x.ToString(), t == null ? "Y" : t.y.ToString());
    }
}
