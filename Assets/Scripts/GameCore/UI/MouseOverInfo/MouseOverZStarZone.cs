using UnityEngine;
using UnityEngine.UI;
using Pathfinding.ZStar;

public class MouseOverZStarZone : MonoBehaviour
{
    MouseController mouseController;
    Text text;

    // Use this for initialization
    void Start()
    {
        text = GetComponent<Text>();
        if (text == null) {
            Debug.LogError("cant find text component");
            this.enabled = false;
        }

        mouseController = FindObjectOfType<MouseController>();
        if (mouseController == null) {
            Debug.LogError("cant find mouse controller");
            this.enabled = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        Tile t = mouseController.GetTileUnderMouse();
        int x = t == null ? -1 : t.x;
        int y = t == null ? -1 : t.y;

        text.text = string.Format("Zone: {0}[{3}] [{1}:{2}]", t == null ? 0 : PathfindingManagerZ.GetZoneID(t), x, y,
            t == null ? 0 : PathfindingManagerZ.GetZoneLinkId(t));
    }
}
