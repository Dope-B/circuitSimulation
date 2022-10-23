using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Switch : Components
{
    public bool isOn = true;
    public Toggle toggle;
    public Sprite ON;
    public Sprite OFF;
    ComponentManager CM;
    public override void init()
    {
        base.init();
        CM = GameObject.Find("Components").GetComponent<ComponentManager>();
    }
    public override void setFocused(bool a)
    {
        base.setFocused(a);
    }
    public override void setInfo(int type=0)
    {
        //toggle.isOn = !isOn;
    }
    public override void destroy()
    {
        base.destroy();
    }
    public override void setPos(Vector2 pos)
    {
        if (vertexsBox.Count < 2)
        {
            vertexsBox.Add(pos);
            vertexsBox.Add(pos + new Vector2(1, 0));
        }
        else
        {
            vertexsBox[0] = pos;
            switch (this.gameObject.transform.localEulerAngles.z)
            {
                case 0:
                    vertexsBox[1] = pos + new Vector2(1, 0);
                    break;
                case 90:
                    vertexsBox[1] = pos + new Vector2(0, 1);
                    break;
            }
        }
    }
    public override void rotatePos()
    {
        switch (this.gameObject.transform.localEulerAngles.z)
        {
            case 90:

                vertexsBox[1] = vertexsBox[1] + new Vector2(-1, 1);
                break;
            case 180:

                vertexsBox[1] = vertexsBox[1] + new Vector2(1, -1);
                this.gameObject.transform.localEulerAngles = new Vector3(0, 0, 0);
                break;
        }
    }

    public override void toggleInfo()
    {
        if (!toggle.isOn) { GetComponent<SpriteRenderer>().sprite = OFF; isOn = false; }
        else { GetComponent<SpriteRenderer>().sprite = ON; isOn = true; }
        if (CircuitAnalyzer.isRunning) { CM.setLoop(); }
    }
}
