using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class MouseOverRoomText : MonoBehaviour
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

        text.text = string.Format("RoomID: {0}", t == null || t.room == null ? "0" : World.current.rooms.IndexOf(t.room).ToString());
    }
}
