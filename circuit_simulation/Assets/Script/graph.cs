using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class graph : MonoBehaviour
{
    public Toggle[] toggle;
    LineRenderer[] line;
    List<float>[] graphYPos;
    public MouseControl MC;
    public GameObject targetObject;
    public TMP_InputField intervalText;
    public TextMeshProUGUI maxText;
    int interval = 10;
    // Start is called before the first frame update
    void Start()
    {
        line = new LineRenderer[2];
        line[0] = transform.GetChild(0).GetChild(0).GetComponent<LineRenderer>();
        line[1] = transform.GetChild(0).GetChild(1).GetComponent<LineRenderer>();
        graphYPos = new List<float>[2];
        targetObject = null;
        setInterval();
        maxText.text = "1";
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void reset()
    {
        //DD.ClearOptions();
        resetToggle();
        resetYPos();
        resetScaleY();
        targetObject = null;
    }
    public void setToggle()
    {
        switch (targetObject.name)
        {
            case "resistor(Clone)":
                toggle = new Toggle[2];
                toggle[0] = MC.info[4].transform.GetChild(6).GetComponent<Toggle>();
                toggle[1] = MC.info[4].transform.GetChild(7).GetComponent<Toggle>();
                break;
            case "inductor(Clone)":
                toggle = new Toggle[2];
                toggle[0] = MC.info[6].transform.GetChild(6).GetComponent<Toggle>();
                toggle[1] = MC.info[6].transform.GetChild(7).GetComponent<Toggle>();
                break;
            case "capacitor(Clone)":
                toggle = new Toggle[2];
                toggle[0] = MC.info[5].transform.GetChild(6).GetComponent<Toggle>();
                toggle[1] = MC.info[5].transform.GetChild(7).GetComponent<Toggle>();
                break;
            case "voltage source(Clone)":
                toggle = new Toggle[1];
                toggle[0] = MC.info[1].transform.GetChild(4).GetComponent<Toggle>();
                break;
            case "AC voltage source(Clone)":
                toggle = new Toggle[1];
                toggle[0] = MC.info[3].transform.GetChild(6).GetComponent<Toggle>();
                break;
            case "current source(Clone)":
                toggle = new Toggle[1];
                toggle[0] = MC.info[2].transform.GetChild(4).GetComponent<Toggle>();
                break;
            default:
                targetObject = null;
                return;
        }
    }
    void resetToggle()
    {
        for (int i = 0; i < toggle.Length; i++)
        {
            toggle[i].isOn = false;
        }
        toggle = new Toggle[0];
    }
    public void resetYPos(int type=2)
    {
        switch (type)
        {
            case 0:
                for (int j = 0; j < graphYPos[0].Count; j++)
                {
                    graphYPos[0][j] = 0;
                    line[0].SetPosition(j, new Vector2(line[0].GetPosition(j).x, 0));
                }
                break;
            case 1:
                for (int j = 0; j < graphYPos[1].Count; j++)
                {
                    graphYPos[1][j] = 0;
                    line[1].SetPosition(j, new Vector2(line[1].GetPosition(j).x, 0));
                }
                break;
            case 2:
                for (int i = 0; i < 2; i++)
                {
                    for (int j = 0; j < graphYPos[i].Count; j++)
                    {
                        graphYPos[i][j] = 0;
                        line[i].SetPosition(j, new Vector2(line[i].GetPosition(j).x, 0));
                    }
                }
                break;
        }
    }
    public void onOffLine(int i)
    {
        switch (targetObject.name)
        {
            case "resistor(Clone)": 
            case "inductor(Clone)":
            case "capacitor(Clone)":
                if (toggle[i].isOn) { line[i].gameObject.SetActive(true); }
                else { line[i].gameObject.SetActive(false); }
                break;
            case "voltage source(Clone)":
            case "AC voltage source(Clone)":
            case "current source(Clone)":
                if (toggle[0].isOn) { line[0].gameObject.SetActive(true); }
                else { line[0].gameObject.SetActive(false); }
                break;
            default:
                return;
        }
    }
   
    public void setInterval()
    {
        for (int i = 0; i < 2; i++)
        {
            interval = int.Parse(intervalText.text);
            line[i].positionCount = interval + 1;
            graphYPos[i] = new List<float>();
            line[i].SetPosition(0, new Vector2(-50, 0));
            graphYPos[i].Add(0);
            for (int j = 1; j < line[i].positionCount - 1; j++)
            {
                line[i].SetPosition(j, new Vector2(-50 + ((50 / (float)interval) * j), 0));
                graphYPos[i].Add(0);
            }
            line[i].SetPosition(line[i].positionCount - 1, new Vector2(0, 0));
            graphYPos[i].Add(0);
        }

    }
    public void setIntervalText()
    {
        intervalText.text = interval.ToString();
    }
    public void setGraphYPos(float x, int k = 0)
    {
        graphYPos[k].Add(x);
        graphYPos[k].RemoveAt(0);
        for (int i = 0; i < graphYPos[k].Count; i++)
        {
            line[k].SetPosition(i, new Vector2(line[k].GetPosition(i).x, graphYPos[k][i]));
        }
    }
    public void scalingY(float max)
    {
        if (1.3f * Mathf.Abs(max) >= 0.5f / line[0].gameObject.transform.localScale.y)
        {
            line[0].gameObject.transform.localScale = new Vector3(0.01f, 0.5f / (1.3f * Mathf.Abs(max)));
            maxText.text = (1.3f * Mathf.Abs(max)).ToString("F3");
            line[1].transform.localScale = line[0].transform.localScale;
        }
    }
    public void resetScaleY()
    {
        line[0].transform.localScale = new Vector2(0.01f, 0.5f);
        line[1].transform.localScale = new Vector2(0.01f, 0.5f);
        maxText.text = "1";
    }
}
