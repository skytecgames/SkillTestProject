using UnityEngine;
using System.Collections;

public class AutomaticHorizontalSize : MonoBehaviour
{
    public float childHeight = 30;
    public float childWidth = 150;

    // Use this for initialization
    void Start()
    {
        AdjustSize();
    }    

    public void AdjustSize()
    {
        Vector2 size = gameObject.GetComponent<RectTransform>().sizeDelta;
        size.y = childHeight;// * gameObject.transform.childCount;
        size.x = childWidth * gameObject.transform.childCount;
        gameObject.GetComponent<RectTransform>().sizeDelta = size;
    }
}