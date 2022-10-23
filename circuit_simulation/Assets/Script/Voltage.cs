using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Voltage : Components
{
    float voltage = 10;
    float frequency = 1;
    float phase = 0;
    Vector2 input;//-
    Vector2 output;//+
    public int type = 0;
    public TMP_InputField[] info;
    public TextMeshProUGUI volText;
    public override void init()
    {
        base.init();
        info = new TMP_InputField[3];
    }
    public override void setFocused(bool a)
    {
        base.setFocused(a);
    }

    public override void setInfo(int a=0)
    {
        //nameText.text = OBname;
        if (!CircuitAnalyzer.isRunning||a==1)
        {
            info[0].text = voltage.ToString();
            if (type == 1)
            {
                info[1].text = frequency.ToString();
                info[2].text = phase.ToString();
            }
        }
    }
    
    public override void acceptInfo(int type)
    {
        switch (type)
        {
            case 0: setVoltage(); break;
            case 1: setFrequency();break;
            case 2: setPhaseDifference(); break;
        }
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

    public void setVoltage() { voltage = float.Parse(info[0].text); }
    public void setFrequency() { if (float.Parse(info[1].text) > 0) { frequency = float.Parse(info[1].text); } else { frequency = 1; setInfo(); } }
    public void setPhaseDifference() { phase = float.Parse(info[2].text); }
    public float getVoltage(float time=0) 
    {
        if (type == 0) { return voltage; }
        else { return voltage * Mathf.Sin((2 * Mathf.PI * frequency * time) + (phase * Mathf.Deg2Rad)); }
    }
    public float getMaxVoltage() { return voltage; }
    public float getFrequency() { return frequency; }
    public float getPhase() { return phase; }
}
