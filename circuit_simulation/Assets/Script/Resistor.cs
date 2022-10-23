using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class Resistor : Components
{
    public float voltage = 0;
    public float current = 0;
    public float resistance = 1;
    float capacitance = 0.01f;
    float inductance = 1f;
    public int type=0;
    public float intergral = 0f;
    public float previousVoltage = 0;
    public TMP_InputField info;
    public TextMeshProUGUI[] infoText;
    public override void init()
    {
        base.init();
    }
    public override void setFocused(bool a)
    {
        base.setFocused(a);
        infoText = new TextMeshProUGUI[2];
    }
    public override void setInfo(int a=0)
    {
        //nameText.text = OBname;
        if (!CircuitAnalyzer.isRunning || a == 1)
        {
            switch (type)
            {
                case 0:
                    info.text = resistance.ToString();
                    break;
                case 1:
                    info.text = capacitance.ToString();
                    break;
                case 2:
                    info.text = inductance.ToString();
                    break;
            }
        }
        infoText[0].text = voltage.ToString();
        infoText[1].text = current.ToString();
    }
    public override void acceptInfo(int type)
    {
        switch (type)
        {
            case 0: setResistance(); break;
            case 1: setCapacitance(); break;
            case 2: setInductance(); break;
        }
        printZ();
    }
    public void printZ()
    {
        switch (type)
        {
            case 0:
                info.text = resistance.ToString();
                break;
            case 1:
                info.text = capacitance.ToString();
                break;
            case 2:
                info.text = inductance.ToString();
                break;
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
                /*case 0:
                    vertexsBox[1] = pos + new Vector2(1, 0);
                    break;
                case 90:
                    vertexsBox[1] = pos + new Vector2(0, 1);
                    break;
                case 180:
                    vertexsBox[1] = pos + new Vector2(-1, 0);
                    break;
                case 270:
                    vertexsBox[1] = pos + new Vector2(0, -1);
                    break;*/
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
    public override void resetData()
    {
        voltage = 0;
        current = 0;
        previousVoltage = 0;
    }
    public override void resetText()
    {
        voltage = 0;
        current = 0;
        infoText[0].text = voltage.ToString();
        infoText[1].text = current.ToString();
    }
    public void setResistance(){ resistance = float.Parse(info.text); }
    public void setCapacitance() { capacitance = float.Parse(info.text); }
    public void setInductance() { inductance = float.Parse(info.text); }
    public float getResistance() { return resistance; }
    public float getZ() 
    {
        switch (type)
        {
            case 1:
                return capacitance;
            case 2:
                return inductance;
            default:
                return resistance;
        }
        
    }
}
