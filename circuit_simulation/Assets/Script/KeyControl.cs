using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class KeyControl : MonoBehaviour
{
    public MouseControl MC;
    public List<Toggle> toggle;
    public TabSliding TS;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            TS.scrolling(1);
        }
        else if(Input.GetKeyDown(KeyCode.R))
        {
            if (!toggle[0].isOn) { toggle[0].SetIsOnWithoutNotify(true); }
            else { toggle[0].SetIsOnWithoutNotify(false); }
            toggle[0].onValueChanged.Invoke(true);
        }
        else if (Input.GetKeyDown(KeyCode.V))
        {
            if (!toggle[1].isOn) { toggle[1].SetIsOnWithoutNotify(true); }
            else { toggle[1].SetIsOnWithoutNotify(false); }
            toggle[1].onValueChanged.Invoke(true);
        }
        else if (Input.GetKeyDown(KeyCode.G))
        {
            if (!toggle[2].isOn) { toggle[2].SetIsOnWithoutNotify(true); }
            else { toggle[2].SetIsOnWithoutNotify(false); }
            toggle[2].onValueChanged.Invoke(true);
        }
        else if (Input.GetKeyDown(KeyCode.A))
        {
            if (MC.isDraw) { MC.setMode(MC.mode); }
        }
        else if (Input.GetKeyDown(KeyCode.C))
        {
            if (!toggle[3].isOn) { toggle[3].SetIsOnWithoutNotify(true); }
            else { toggle[3].SetIsOnWithoutNotify(false); }
            toggle[3].onValueChanged.Invoke(true);
        }
        else if (Input.GetKeyDown(KeyCode.W))
        {
            if (!toggle[4].isOn) { toggle[4].SetIsOnWithoutNotify(true); }
            else { toggle[4].SetIsOnWithoutNotify(false); }
            toggle[4].onValueChanged.Invoke(true);
        }
        else if (Input.GetKeyDown(KeyCode.Delete))
        {
            if (MC.focusedObject != null) { MC.destroyFocuedObject(); }
        }
        else if (Input.GetKeyDown(KeyCode.Escape)) { Application.Quit(); }
    }
}
