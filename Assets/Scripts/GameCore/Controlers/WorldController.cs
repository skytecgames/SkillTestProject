using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.SceneManagement;
using System.Xml.Serialization;
using System.IO;

public class WorldController : MonoBehaviour
{
    public static WorldController Instance { get; protected set; }    

    protected World world;// { get; protected set; }

    static bool loadWorld = false;

    private bool pause = false;

	// Use this for initialization
	void Awake () {
        Instance = this;

        if (loadWorld) {
            loadWorld = false;
            CreateWorldFromSaveData("SaveGame00");
        } else {
            CreateEmptyWorld();
        }

        FindObjectOfType<InputController>().OnPauseEvent += OnPauseFunc;
    }

    private void Update()
    {
        if (pause == false) {
            world.Update(Time.deltaTime);
        }
    }

    private void OnPauseFunc()
    {
        pause =! pause;
    }

    public static Tile GetTileAtWorldCoord(Vector3 coord)
    {
        int x = Mathf.FloorToInt(coord.x + 0.5f);
        int y = Mathf.FloorToInt(coord.y + 0.5f);

        return Instance.world[x, y];        
    }            

    public void NewWorld()
    {
        Debug.Log("NewWorld");

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void SaveWorld()
    {
        SaveWorld("SaveGame00");
    }

    public void SaveWorld(string name)
    {
        Debug.Log("SaveWorld");        

        MiniJSON.JSONObject json = new MiniJSON.JSONObject();
        world.Write(json);

        string data = json.ToString();

        Debug.Log(data);

        PlayerPrefs.SetString(name, data);
    }

    public void LoadWorld()
    {
        Debug.Log("LoadWorld");

        loadWorld = true;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    void CreateEmptyWorld()
    {
        world = new World(100, 100);

        Camera.main.transform.position = new Vector3(world.width / 2, world.height / 2, Camera.main.transform.position.z);
    }

    void CreateWorldFromSaveData(string name)
    {
        Debug.Log(PlayerPrefs.GetString(name));

        MiniJSON.JSONObject json = MiniJSON.JSONObject.Parse(PlayerPrefs.GetString(name));
        world = new World();
        world.Read(json);

        Camera.main.transform.position = new Vector3(world.width / 2, world.height / 2, Camera.main.transform.position.z);
    }
}
