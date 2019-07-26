using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class MouseOverItemText : MonoBehaviour
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

        if (t == null || t.itemContainer == null || t.itemContainer.IsEmpty()) {
            text.text = "Item: Null";
        } else {
            Item i = t.itemContainer.GetTopItem();
            text.text = string.Format("Item: {0}[{1}/{2}]", i.objectType, i.stackSize, i.maxStackSize);
        }
    }
}
