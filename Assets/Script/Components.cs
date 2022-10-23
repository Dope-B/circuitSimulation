using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Components : MonoBehaviour
{
    [System.Serializable]
    public class Vertex
    {
        public Vector2 vertex;
        public List<Components> linkedObject;
        public ComponentManager.Node linkedNode;
    }
    public List<Vertex> Vertexs;
    public List<Vector2> vertexsBox;
    public List<Components> linkedObject;
    public bool isFocused;
    public bool isCollided = false;

    public virtual void init() { }
    public virtual void setFocused(bool a)
    {
        isFocused = a;
        if (isFocused) { this.gameObject.GetComponent<SpriteRenderer>().color -= new Color(0, 0, 0, 0.5f); }
        else { this.gameObject.GetComponent<SpriteRenderer>().color += new Color(0, 0, 0, 0.5f); }
    }
    public virtual void destroy() 
    {
        setFocused(false);
        for (int i = 0; i < linkedObject.Count; i++)
        {
            linkedObject[i].linkedObject.Remove(this);
        }
        /*for (int i = 0; i < Vertexs.Count; i++)
        {
            for (int j = 0; j < Vertexs[i].linkedObject.Count; j++)
            {
                for (int k = 0; k < Vertexs[i].linkedObject[j].Vertexs.Count; k++)
                {
                    if (Vertexs[i].linkedObject[j].Vertexs[k].linkedObject.Contains(this)) { Vertexs[i].linkedObject[j].Vertexs[k].linkedObject.Remove(this); }
                }
            }
        }*/
        Destroy(this.gameObject);
    }

    public virtual void function() { }

    public virtual void setPos(Vector2 pos) { }

    public virtual void rotatePos() { }

    public virtual void setInfo(int type=0) {  }
    public virtual void acceptInfo(int type) { }
    public virtual void toggleInfo() { }
    public virtual void resetData() { }
    public virtual void resetText() { }
    public void resetLinkedObject()
    {
        linkedObject = new List<Components>();
        //List<Components> listForRemove = linkedObject;
        linkedObject = getLinkedObject();
        /*for (int i = 0; i < listForRemove.Count; i++)
        {
            if (!linkedObject.Contains(listForRemove[i])) { listForRemove[i].linkedObject.Remove(this); listForRemove[i].setVertexs(); }
        }*/
        setVertexs();
    }
    public List<Components> getLinkedObject()
    {
        List<Components> LO = new List<Components>();
        for (int i = 0; i < vertexsBox.Count; i++)
        {
            for (int j = 0; j < MouseControl.componentManager.compo.Count; j++)
            {
                for (int k = 0; k < MouseControl.componentManager.compo[j].vertexsBox.Count; k++)
                {
                    if (vertexsBox[i] == MouseControl.componentManager.compo[j].vertexsBox[k] && MouseControl.componentManager.compo[j] != this)
                    {
                        LO.Add(MouseControl.componentManager.compo[j]);
                        if (!MouseControl.componentManager.compo[j].linkedObject.Contains(this)) { MouseControl.componentManager.compo[j].linkedObject.Add(this); /*MouseControl.componentManager.compo[j].setVertexs();*/ }
                    }
                }
            }
        }
        return LO;
    }

    public void setVertexs()
    {
        Vertexs = new List<Vertex>();
        for (int i = 0; i < vertexsBox.Count; i++)
        {
            Vertex box = new Vertex();
            box.linkedObject = new List<Components>();
            //box.linkedNode = new ComponentManager.Node();
            box.vertex = vertexsBox[i];
            for (int j = 0; j < linkedObject.Count; j++)
            {
                for (int k = 0; k < linkedObject[j].vertexsBox.Count; k++)
                {
                    if (vertexsBox[i] == linkedObject[j].vertexsBox[k]) { box.linkedObject.Add(linkedObject[j]); }
                }
            }
            Vertexs.Add(box);
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (this.gameObject.tag == "wire" && collision.gameObject.tag == "wire") { isCollided = true; }
        else if (collision.gameObject.tag == "components" && isFocused)
        {
            if ((((Mathf.Abs(this.transform.localRotation.eulerAngles.z - collision.gameObject.transform.localRotation.eulerAngles.z)) / 90) % 2 != 1)) { isCollided = true; }
            else { isCollided = false; }
        }
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.collider.gameObject.tag == "components") { isCollided = false; }
        else if (this.gameObject.tag == "wire" && collision.gameObject.tag == "wire") { isCollided = false; }
    }
}
