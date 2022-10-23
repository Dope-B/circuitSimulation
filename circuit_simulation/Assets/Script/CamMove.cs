using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamMove : MonoBehaviour
{
    public static Camera cam;
    public GameObject grid;
    public float zSpeed = 0.5f;
    public float maxZoom;
    public float minZoom;
    float halfWidth;
    float halfHeight;
    Vector3 CMgap;
    Vector3 originPos;
    bool isCamClicked;
    // Start is called before the first frame update
    void Start()
    {
        cam = GetComponent<Camera>();
        halfHeight = cam.orthographicSize;
        halfWidth = cam.orthographicSize * Screen.width / Screen.height;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void zoomIn() { cam.orthographicSize = Mathf.Max(maxZoom, cam.orthographicSize - zSpeed);limitCamBound(); }
    public void zoomOut(){ cam.orthographicSize = Mathf.Min(minZoom, cam.orthographicSize + zSpeed); limitCamBound(); }
    public void camDrag(bool b1)
    {
        if (b1)
        {
            CMgap = cam.transform.position - cam.ScreenToWorldPoint(Input.mousePosition);
            if (!isCamClicked)
            {
                originPos = cam.ScreenToWorldPoint(Input.mousePosition);
                isCamClicked = true;
            }
        }
        else { isCamClicked = false; }
        if (isCamClicked)
        {
            cam.transform.position = originPos + CMgap;
            float x1 = Mathf.Clamp(originPos.x + CMgap.x,
                                   grid.gameObject.GetComponent<BoxCollider2D>().bounds.min.x + halfWidth,
                                   grid.gameObject.GetComponent<BoxCollider2D>().bounds.max.x - halfWidth);
            float y1 = Mathf.Clamp(originPos.y + CMgap.y,
                                   grid.gameObject.GetComponent<BoxCollider2D>().bounds.min.y + halfHeight,
                                   grid.gameObject.GetComponent<BoxCollider2D>().bounds.max.x - halfHeight);
            cam.transform.position = new Vector3(x1, y1, (originPos.z + CMgap.z));
        }
    }
    void limitCamBound()
    {
        halfHeight = cam.orthographicSize;
        halfWidth = cam.orthographicSize * Screen.width / Screen.height;
        float x1 = Mathf.Clamp(transform.position.x,
                                   grid.gameObject.GetComponent<BoxCollider2D>().bounds.min.x + halfWidth,
                                   grid.gameObject.GetComponent<BoxCollider2D>().bounds.max.x - halfWidth);
        float y1 = Mathf.Clamp(transform.position.y,
                               grid.gameObject.GetComponent<BoxCollider2D>().bounds.min.y + halfHeight,
                               grid.gameObject.GetComponent<BoxCollider2D>().bounds.max.x - halfHeight);
        cam.transform.position = new Vector3(x1, y1, transform.position.z);
    }
}
