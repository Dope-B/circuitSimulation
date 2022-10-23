using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Info : MonoBehaviour
{
    public MouseControl MC;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    public void objectDestroy()
    {
        if (!CircuitAnalyzer.isRunning) { MC.destroyFocuedObject(); }
    }
    public void rotate()
    {
        if (!CircuitAnalyzer.isRunning) { StartCoroutine(Irotate()); }
    }

    IEnumerator Irotate()
    {
        MC.focusedObject.transform.localRotation = MC.focusedObject.transform.localRotation * Quaternion.Euler(0, 0, 90);
        MC.focusedObject.GetComponent<Components>().rotatePos();
        yield return new WaitForSeconds(0.05f);
        if (MC.focusedObject.GetComponent<Components>().isCollided) { StartCoroutine(Irotate()); }
        //MC.focusedObject.GetComponent<Components>().getLinkedObject();
    }
    public void acceptButton(int type)
    {
        MC.focusedObject.GetComponent<Components>().acceptInfo(type);
    }
    public void toggleButton() { MC.focusedObject.GetComponent<Components>().toggleInfo(); }
    public void setWireColor(GameObject g) { MC.focusedObject.GetComponent<WireController>().setColor(g.GetComponent<Image>().color - new Color(0, 0, 0, 0.25f)); }
}
