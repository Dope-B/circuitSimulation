using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WireController : Components
{
    public List<Wire> wires;
    public GameObject wire;
    public Color color;
    public float width;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    public override void init()
    {
        width = 0.1f;
        color = Color.blue;
        setColor(color);
    }
    public void setColor(Color c)
    {
        if (isFocused) { color = c + new Color(0, 0, 0, 0.25f); }
        else { color = c; }
        for (int i = 0; i < wires.Count; i++) { wires[i].setColor(c); }
    }
    public void addWire()
    {
        GameObject Iwire = Instantiate(wire);
        Iwire.transform.SetParent(this.transform);
        Iwire.GetComponent<Wire>().init();
        Iwire.GetComponent<Wire>().setColor(color);
        wires.Add(Iwire.GetComponent<Wire>());
    }
    public void addVertex(Vector2 Vertex)
    {
        vertexsBox.Add(Vertex);
        if (vertexsBox.Count > 1) { resetPos(); }
    }
    void removeWire()
    {
        wires[wires.Count - 1].destroy();
        resetPos();
    }
    public void resetPos()
    {
        for (int i = 0; i < wires.Count; i++)
        {
            wires[i].setVertex(vertexsBox[i], vertexsBox[i + 1]);
        }
        transform.position = vertexsBox[0];
    }
    public void moveVertex(Vector2 pos, int i)
    {
        vertexsBox[i] = pos;
        resetPos();
    }
    public void insertVertex(Vector2 Vertex, int i)
    {
        if (!vertexsBox.Contains(Vertex))
        {
            vertexsBox.Insert(i, Vertex);
            addWire();
            resetPos();
        }
    }
    /*public bool vertexClickedCheck()
    {
        for (int i = 0; i < vertex.Count; i++) // Is vertex clicked
        {
            if (Vector2.Distance(CamMove.cam.ScreenToWorldPoint(Input.mousePosition), vertex[i]) <= 0.25f)
            {
                return true;
            }
        }
        return false;
    }*/
    public void removeVertex(int i)
    {
        vertexsBox.RemoveAt(i);
        removeWire();
    }
    public override void setFocused(bool a)
    {
        isFocused = a;
        for (int i = 0; i < wires.Count; i++)
        {
            wires[i].setFocused(a);
        }
    }
    public void setCollider()
    {
        for (int i = 0; i < wires.Count; i++)
        {
            wires[i].setCollider();
        }
    }

    public bool vertexDeleteCheck(int k)
    {
        for (int i = 0; i < vertexsBox.Count; i++) //overlap check
        {
            if (i != k)
            {
                if (vertexsBox[i] == vertexsBox[k]) { return true; }
            }
        }
        if (k != vertexsBox.Count - 1 && k != 0) // gradient check
        {
            if (((vertexsBox[k] - vertexsBox[k - 1]).normalized == (vertexsBox[k + 1] - vertexsBox[k]).normalized) || ((vertexsBox[k] - vertexsBox[k - 1]).normalized == (vertexsBox[k] - vertexsBox[k + 1]).normalized)) { return true; }
        }
        /*else if (k == 0)
        {
            if (vertex.Count > 2) { if (((vertex[k] - vertex[k + 1]).normalized == (vertex[k + 2] - vertex[k]).normalized) || ((vertex[k] - vertex[k + 1]).normalized == (vertex[k] - vertex[k + 2]).normalized)) { return true; } }
        }
        else if (k == vertex.Count - 1)
        {
            if (vertex.Count > 2) { if (((vertex[k] - vertex[k - 1]).normalized == (vertex[k - 2] - vertex[k]).normalized) || ((vertex[k] - vertex[k - 1]).normalized == (vertex[k] - vertex[k - 2]).normalized)) { return true; } }
        }*/
        return false;
    }

    public int lineClicked(Vector2 v1)
    {
        for (int i = 0; i < vertexsBox.Count - 1; i++)
        {
            Vector2 a1 = vertexsBox[i + 1] - vertexsBox[i];
            Vector2 a2 = v1 - vertexsBox[i];
            float a3 = Vector3.Magnitude(Vector3.Cross(a1, a2)) / Vector3.Magnitude(a1);
            if (a3 <= width * 0.6f&& Vector3.Magnitude(a1)> Vector3.Magnitude(a2)) { return i + 1; }
        }
        return 100;
    }
    public int vertexClicked(Vector2 v1)
    {
        for (int i = 0; i < vertexsBox.Count; i++) // Is vertex clicked
        {
            if (Vector2.Distance(v1, vertexsBox[i]) <= 0.25f) { return i; }
        }
        return 100;
    }
}
