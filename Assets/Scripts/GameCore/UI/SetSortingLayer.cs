using UnityEngine;
using System.Collections;

public class SetSortingLayer : MonoBehaviour
{
    public string sortingLayerName = "Default";

    // Use this for initialization
    void Start()
    {
        Renderer r = GetComponent<Renderer>();
        if (r != null) {
            r.sortingLayerName = sortingLayerName;
        }

        Component.Destroy(this);
    }    
}
