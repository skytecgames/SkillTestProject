using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuildMenu : MonoBehaviour
{
    public GameObject prefab;

    // Use this for initialization
    void Start()
    {
        BuildModeController bmc = FindObjectOfType<BuildModeController>();

        foreach (string obj in World.current.installedObjectCollection.GetIterator())
        {
            GameObject go = Instantiate(prefab, this.gameObject.transform);
            go.name = string.Format(obj);
            Text txt = go.GetComponentInChildren<Text>();
            txt.text = obj;

            string obj_type = obj;
            go.GetComponent<Button>().onClick.AddListener( () => { bmc.SetMode_BuildInstalledObject(obj_type); } );
        }

        GetComponent<AutomaticVerticalSize>().AdjustSize();
    }

    public void Show()
    {
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
