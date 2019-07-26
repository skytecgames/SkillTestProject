using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class MouseOverRoomDetailsText : MonoBehaviour
{
    MouseController mouseController;
    Text text;

    // Use this for initialization
    void Start()
    {
        text = GetComponent<Text>();
        if (text == null)
        {
            Debug.LogError("cant find text component");
            this.enabled = false;
        }

        mouseController = FindObjectOfType<MouseController>();
        if (mouseController == null)
        {
            Debug.LogError("cant find mouse controller");
            this.enabled = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        Tile t = mouseController.GetTileUnderMouse();
        string s = "";

        if (t != null && t.room != null) {
            foreach (string g in t.room.GetGasNames()) {
                s += string.Format("{1}  {2}: {0:0.00} ({3:0}%)", t.room.GetGasAmount(g), s.Length == 0 ? "" : "\n", g, t.room.GetGasPercentage(g) * 100f);
            }
        }

        text.text = s;
    }
}
