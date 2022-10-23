using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineWidthScaling : MonoBehaviour
{
    LineRenderer line;
    public float targetWidth;
    // Start is called before the first frame update
    void Start()
    {
        line = GetComponent<LineRenderer>();
        line.startWidth=targetWidth;
        line.endWidth = targetWidth;
    }

    // Update is called once per frame
    void Update()
    {
        if (line.startWidth != CamMove.cam.orthographicSize / (3 / targetWidth))
        {
            line.startWidth = CamMove.cam.orthographicSize / (3 / targetWidth);
            line.endWidth = CamMove.cam.orthographicSize / (3 / targetWidth);
        }
    }
}
