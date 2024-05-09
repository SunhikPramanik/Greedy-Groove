using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseScrollZoom : MonoBehaviour
{
    Camera cameraSize;
    public float variableSize = 0.25f;

    // Start is called before the first frame update
    void Start()
    {
        cameraSize = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.mouseScrollDelta.y < 0)
        {
            cameraSize.orthographicSize += variableSize;
        }
        else if(Input.mouseScrollDelta.y > 0 && cameraSize.orthographicSize >= 1.25f)
        {
            cameraSize.orthographicSize -= variableSize;
        }
    }
}
