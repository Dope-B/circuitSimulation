using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Current : Components
{
    [SerializeField]
    public float current;
    public TMP_InputField info;
    Vector2 input;
    Vector2 output;
    public override void init()
    {
        base.init();
        
    }
    public override void setFocused(bool a)
    {
        base.setFocused(a);
    }
    public override void setInfo(int a=0)
    {
        //nameText.text = OBname;
        if (!CircuitAnalyzer.isRunning || a == 1) { info.text = current.ToString(); }
        
    }
    public override void acceptInfo(int type)
    {
        setCurrent();
    }
    public override void destroy()
    {
        base.destroy();
    }
    public override void setPos(Vector2 pos)
    {
        if (vertexsBox.Count < 2)
        {
            vertexsBox.Add(pos + new Vector2(-1, 0));
            vertexsBox.Add(pos + new Vector2(1, 0));
        }
        else
        {
            switch (this.gameObject.transform.localEulerAngles.z)
            {
                case 0:
                    vertexsBox[0] = pos + new Vector2(-1, 0);
                    vertexsBox[1] = pos + new Vector2(1, 0);
                    break;
                case 90:
                    vertexsBox[0] = pos + new Vector2(0, -1);
                    vertexsBox[1] = pos + new Vector2(0, 1);
                    break;
                case 180:
                    vertexsBox[0] = pos + new Vector2(1, 0);
                    vertexsBox[1] = pos + new Vector2(-1, 0);
                    break;
                case 270:
                    vertexsBox[0] = pos + new Vector2(0, 1);
                    vertexsBox[1] = pos + new Vector2(0, -1);
                    break;
            }
        }
        input = vertexsBox[0];
        output = vertexsBox[1];
    }
    public override void rotatePos()
    {
        switch (this.gameObject.transform.localEulerAngles.z)
        {
            case 0:
                vertexsBox[0] = vertexsBox[0] + new Vector2(-1, -1);
                vertexsBox[1] = vertexsBox[1] + new Vector2(1, 1);
                break;
            case 90:
                vertexsBox[0] = vertexsBox[0] + new Vector2(1, -1);
                vertexsBox[1] = vertexsBox[1] + new Vector2(-1, 1);
                break;
            case 180:
                vertexsBox[0] = vertexsBox[0] + new Vector2(1, 1);
                vertexsBox[1] = vertexsBox[1] + new Vector2(-1, -1);
                break;
            case 270:
                vertexsBox[0] = vertexsBox[0] + new Vector2(-1, 1);
                vertexsBox[1] = vertexsBox[1] + new Vector2(1, -1);
                break;
        }
        input = vertexsBox[0];
        output = vertexsBox[1];
    }

    public void setCurrent() { current =  float.Parse(info.text); }
    public float getCurrent() { return current; }
}
