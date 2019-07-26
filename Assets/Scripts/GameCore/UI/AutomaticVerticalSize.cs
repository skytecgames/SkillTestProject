using UnityEngine;
using System.Collections;

public class AutomaticVerticalSize : MonoBehaviour
{
    public float childHeight = 30;

    // Use this for initialization
    void Start()
    {
        AdjustSize();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void AdjustSize()
    {
        Vector2 size = gameObject.GetComponent<RectTransform>().sizeDelta;
        size.y = childHeight * gameObject.transform.childCount;
        gameObject.GetComponent<RectTransform>().sizeDelta = size;
    }
}
