using UnityEngine;
using System.Collections;

public class InputController : MonoBehaviour
{
    public event System.Action OnPauseEvent;

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space)) {
            if(OnPauseEvent != null) {
                OnPauseEvent();
            }
        }
    }
}
