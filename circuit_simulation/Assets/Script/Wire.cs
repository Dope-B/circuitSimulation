using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wire : Components
{
    public WireController wc;
    LineRenderer lineRenderer;
    PolygonCollider2D col;
    public Color color;
    private float width;
    float a1 = 0;
    float x = 0;
    float y = 0;
    // Start is called before the first frame update

    // Update is called once per frame
    void Update()
    {

    }

    public override void init()
    {
        wc = this.gameObject.transform.parent.gameObject.GetComponent<WireController>();
        lineRenderer = GetComponent<LineRenderer>();
        col = GetComponent<PolygonCollider2D>();
        width = lineRenderer.endWidth;
        color = lineRenderer.startColor;
        lineRenderer.positionCount = 2;
        vertexsBox.Add(Vector2.zero);
        vertexsBox.Add(Vector2.zero);
        resetPos();
    }
    public override void destroy()
    {
        wc.wires.Remove(this);
        Destroy(this.gameObject);
    }
    public void resetPos()
    {
        for (int i = 0; i < vertexsBox.Count; i++)
        {
            lineRenderer.SetPosition(i, vertexsBox[i]);
        }
        //transform.position = Vector2.zero;
    }

    public void setColor(Color c)
    {
        color = c;
        lineRenderer.startColor = color;
        lineRenderer.endColor = color;
    }
    public void setCollider()
    {
        List<Vector2> point = new List<Vector2>();
        /*if (vertexs.Count > 2)
        {
            Vector2 v1 = getNormalVector(vertexs[0], vertexs[1], vertexs[0]);
            getColPoint(v1);
            point.Add(vertexs[0] + new Vector2(x, y));
            point.Add(vertexs[0] - new Vector2(x, y));
            for (int i = 1; i < vertexs.Count - 1; i++)
            {
                v1 = getNormalVector(vertexs[i - 1], vertexs[i + 1], vertexs[i]);
                getColPoint(v1);
                point.Add(vertexs[i] - new Vector2(x, y));
            }

            v1 = getNormalVector(vertexs[vertexs.Count - 2], vertexs[vertexs.Count - 1], vertexs[vertexs.Count - 1]);
            getColPoint(v1);
            point.Add(vertexs[vertexs.Count - 1] - new Vector2(x, y));
            point.Add(vertexs[vertexs.Count - 1] + new Vector2(x, y));
            for (int i = 2; i < vertexs.Count; i++)
            {
                v1 = getNormalVector(vertexs[vertexs.Count - (i - 1)], vertexs[vertexs.Count - (i + 1)], vertexs[vertexs.Count - i]);
                getColPoint(v1);
                point.Add(vertexs[vertexs.Count - i] + new Vector2(x, y));
            }
        }
        else
        {
            
        }*/
        Vector2 v1 = getNormalVector(vertexsBox[0], vertexsBox[1], vertexsBox[0]);
        getColPoint(v1);
        point.Add(vertexsBox[0] + new Vector2(x, y));
        point.Add(vertexsBox[0] - new Vector2(x, y));
        point.Add(vertexsBox[1] - new Vector2(x, y));
        point.Add(vertexsBox[1] + new Vector2(x, y));
        col.SetPath(0, point);
        transform.position = Vector2.zero;
    }
    public override void setFocused(bool a)
    {
        if (a) { setColor(color - new Color(0, 0, 0, 0.25f)); }
        else { setColor(color + new Color(0, 0, 0, 0.25f)); }
    }

    Vector2 getNormalVector(Vector2 from, Vector2 to, Vector2 target)
    {
        Vector2 v1 = (to - target).normalized - (from - target).normalized;
        v1 = new Vector3(v1.y, -v1.x);
        return v1;
    }
    void getColPoint(Vector2 v1)
    {
        if (v1.x == 0)
        {
            a1 = 0;
            x = 0;
            y = (width * 0.6f);
        }
        else
        {
            a1 = v1.y / v1.x;
            x = (width * 0.6f) / Mathf.Sqrt(Mathf.Pow(a1, 2) + 1);
            y = a1 * x;

        }
    }
    public void setVertex(Vector2 from, Vector2 to)
    {
        vertexsBox[0] = from;
        vertexsBox[1] = to;
        resetPos();
    }
}
