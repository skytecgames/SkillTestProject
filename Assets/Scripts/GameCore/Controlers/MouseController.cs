using UnityEngine;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public class MouseController : MonoBehaviour
{
    public GameObject cursor;

    Vector3 currentMousePosition;
    Vector3 lastMousePosition;

    Vector3 dragStartPosition;
    List<GameObject> dragPreviewGameobjects = new List<GameObject>();
    List<GameObject> dragPreviewInstalledObjects = new List<GameObject>();

    BuildModeController buildModeController;
    InstalledObjectSpriteController installedObjectSpriteController;

    public bool objectTypeNotDragable = false;
    bool isDragging = false;

    enum MouseMode {
        SELECT,
        BUILD
    }

    MouseMode currentMode;

    private void Start()
    {
        buildModeController = GameObject.FindObjectOfType<BuildModeController>();
        installedObjectSpriteController = GameObject.FindObjectOfType<InstalledObjectSpriteController>();
    }

    // Update is called once per frame
    void Update()
    {
        currentMousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        currentMousePosition.z = 0;

        if(Input.GetKeyDown(KeyCode.Escape))
        {
            if(currentMode == MouseMode.BUILD) {
                currentMode = MouseMode.SELECT;                
            } else {
                Debug.Log("show game menu??");
            }
        }       

        UpdateDrag();

        UpdateCameraMovement();

        lastMousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        lastMousePosition.z = 0;
    }

    public Tile GetTileUnderMouse()
    {        
        return WorldController.GetTileAtWorldCoord(currentMousePosition);
    }

    //Функция управления движением камеры
    void UpdateCameraMovement()
    {
        if (Input.GetMouseButton(1)) {
            Vector3 diff = lastMousePosition - currentMousePosition;
            Camera.main.transform.Translate(diff);
        }

        Camera.main.orthographicSize -= Camera.main.orthographicSize * Input.GetAxis("Mouse ScrollWheel");
        Camera.main.orthographicSize = Mathf.Clamp(Camera.main.orthographicSize, 5f, 20f);
    }

    void UpdateDrag()
    {
        if(EventSystem.current.IsPointerOverGameObject()) {
            return;
        }

        ClearPreview();

        if(currentMode != MouseMode.BUILD) {
            return;
        }

        //start drag
        if (Input.GetMouseButtonDown(0)) {
            dragStartPosition = currentMousePosition;
            isDragging = true;
        } else if(isDragging == false) {
            dragStartPosition = currentMousePosition;
        }        

        if(objectTypeNotDragable) {
            dragStartPosition = currentMousePosition;
        }

        if (Input.GetMouseButtonDown(1) && isDragging) {
            isDragging = false;
        } else if(Input.GetMouseButtonDown(1)) {
            currentMode = MouseMode.SELECT;
        }

        int start_x = Mathf.FloorToInt(dragStartPosition.x + 0.5f);
        int end_x = Mathf.FloorToInt(currentMousePosition.x + 0.5f);
        int start_y = Mathf.FloorToInt(dragStartPosition.y + 0.5f);
        int end_y = Mathf.FloorToInt(currentMousePosition.y + 0.5f);

        if (end_x < start_x) {
            int tmp = start_x;
            start_x = end_x;
            end_x = tmp;
        }        
        if (end_y < start_y) {
            int tmp = start_y;
            start_y = end_y;
            end_y = tmp;            
        }        

        for (int x = start_x; x <= end_x; ++x) {
            for (int y = start_y; y <= end_y; ++y) {
                Tile t = World.current[x, y];
                if (t != null) {                                                   
                    if (buildModeController.buildMode == BuildMode.OBJECT) {
                        ShowInstalledObjectPreviewAtTile(buildModeController.buildModeObjectType, t);
                    } else {
                        GameObject go = SimplePool.Spawn(cursor, new Vector3(x, y, 0), Quaternion.identity);
                        dragPreviewGameobjects.Add(go);
                        go.transform.SetParent(transform, true);
                    }
                }
            }
        }        

        //end drag
        if (isDragging && Input.GetMouseButtonUp(0)) {
            isDragging = false;
            for (int x = start_x; x <= end_x; ++x) {
                for (int y = start_y; y <= end_y; ++y) {
                    Tile t = World.current[x, y];
                    if (t != null) {                        
                        buildModeController.DoBuild(t);
                    }
                }
            }
        }
    }

    // ------ ОТОБРАЖЕНИЕ ПРЕВЬЮ ДЛЯ СОЗДАВАЕМОГО ОБЪЕКТА ------           

    void ShowInstalledObjectPreviewAtTile(string objectType, Tile t)
    {
        GameObject go = new GameObject();
        dragPreviewInstalledObjects.Add(go);

        SpriteRenderer go_sr = go.AddComponent<SpriteRenderer>();
        go_sr.sprite = installedObjectSpriteController.GetSpriteForInstalledObject(objectType);
        go_sr.sortingLayerName = "MapUI";

        if (World.current.IsInstalledObjectPlacementValid(objectType, t) == false) {
            //continue;
            go_sr.color = new Color(1f, 0.5f, 0.5f, 0.25f);
        } else {
            go_sr.color = new Color(0.5f, 1f, 0.5f, 0.25f);
        }

        InstalledObject obj = World.current.installedObjectCollection.GetPrototype(objectType);
        int width = obj != null ? obj.width : 1;
        int height = obj != null ? obj.height : 1;
        go.transform.position = new Vector3(t.x + (width - 1) / 2f, t.y + (height - 1) / 2f, 0);

        go.SetActive(true);
    }

    // ---------------------------------------------------------

    public void StartBuildMode()
    {
        currentMode = MouseMode.BUILD;
    }

    void ClearPreview()
    {
        while (dragPreviewGameobjects.Count > 0)
        {
            GameObject go = dragPreviewGameobjects[0];
            dragPreviewGameobjects.RemoveAt(0);
            SimplePool.Despawn(go);
        }

        while (dragPreviewInstalledObjects.Count > 0)
        {
            GameObject go = dragPreviewInstalledObjects[0];
            dragPreviewInstalledObjects.RemoveAt(0);
            GameObject.Destroy(go);
        }
    }
}
