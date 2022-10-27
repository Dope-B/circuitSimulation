using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MouseControl : MonoBehaviour
{
    public GameObject menuScroll;
    public static ComponentManager componentManager;
    public graph graph;
    public CircuitAnalyzer CA;
    public List<GameObject> info;
    public Toggle selectedMenuToggle = null;
    public List<GameObject> components;
    public int mode;//0-select, 1-wire.....16-XNOR
    int focusedWireVertex = -1;
    public bool isDraw = false;// 1-drawMode, 0-selectMode
    bool isDragComponent = false;
    public GameObject focusedObject = null;
    RaycastHit2D hit;
    Vector2 gap;

    // Start is called before the first frame update
    void Start()
    {
        componentManager = GameObject.Find("Components").GetComponent<ComponentManager>();
    }

    // Update is called once per frame
    void Update()
    {
        Wheel();
        RDrag();
        LClick();
        //setResolution();
    }

    void Wheel()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll > 0)
        {
            if (!UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject()) { CamMove.cam.GetComponent<CamMove>().zoomIn(); }
            else
            {
                hit = Physics2D.Raycast(CamMove.cam.ScreenToWorldPoint(Input.mousePosition), Vector2.zero, 0f, 1 << 5);
                if (hit)
                {
                    switch (hit.collider.gameObject.tag)
                    {
                        case "menu":
                            menuScroll.GetComponent<Scrollbar>().value = Mathf.Min(1, menuScroll.GetComponent<Scrollbar>().value + 0.1f);
                            break;
                        default: break;
                    }
                }
                else { }
            }
        }
        else if (scroll < 0)
        {
            if (!UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject()) { CamMove.cam.GetComponent<CamMove>().zoomOut(); }
            else
            {
                hit = Physics2D.Raycast(CamMove.cam.ScreenToWorldPoint(Input.mousePosition), Vector2.zero, 0f, 1 << 5);
                if (hit)
                {
                    switch (hit.collider.gameObject.tag)
                    {
                        case "menu":
                            menuScroll.GetComponent<Scrollbar>().value = Mathf.Max(0, menuScroll.GetComponent<Scrollbar>().value - 0.1f);
                            break;
                        default: break;
                    }
                }
                else { }
            }
        }
    }
    void LClick()
    {
        if (!UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
        {
            if (isDraw&&!CircuitAnalyzer.isRunning)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    if (focusedObject != null) { setFocusedObjectNull(); }
                    if (!objectOverlapCheck())
                    {
                        switch (mode)
                        {
                            case 1:
                                dragWire(0);
                                break;
                            default:
                                dragComponents(0, mode, Vector2.zero);
                                break;
                        }
                    }
                    else
                    {
                        if (focusedObject != null)
                        {
                            setFocusedObjectNull();
                        }
                    }

                }
                if (Input.GetMouseButton(0))
                {
                    if (focusedObject != null)
                    {
                        switch (mode)
                        {
                            case 1:
                                dragWire(1, 1);
                                break;
                            default:
                                dragComponents(1, mode, Vector2.zero);
                                break;
                        }
                    }
                }
                if (Input.GetMouseButtonUp(0))
                {
                    if (focusedObject != null)
                    {
                        switch (mode)
                        {
                            case 1:
                                if (focusedObject != null)
                                {
                                    dragWire(2);
                                }
                                break;
                            default:
                                dragComponents(2, mode, Vector2.zero);
                                break;
                        }
                    }
                }
            }
            else
            {
                if (Input.GetMouseButtonDown(0))
                {
                    hit = Physics2D.Raycast(CamMove.cam.ScreenToWorldPoint(Input.mousePosition), Vector2.zero, 0f, 1 << 3);
                    if (hit)
                    {
                        if (focusedObject != null) { setFocusedObjectNull(); }
                        focusedObject = hit.collider.gameObject;
                        if (hit.collider.gameObject.tag != "wire")
                        {
                            focusedObject.GetComponent<Components>().setFocused(true);
                            setGraph(focusedObject);
                            setInfo(true);
                            gap = -focusedObject.transform.position + CamMove.cam.ScreenToWorldPoint(Input.mousePosition);
                        }
                        switch (hit.collider.gameObject.tag)
                        {
                            case "wire":
                                focusedObject = focusedObject.transform.parent.gameObject;
                                isDragComponent = true;
                                setInfo(true);
                                int a1 = focusedObject.GetComponent<WireController>().vertexClicked(CamMove.cam.ScreenToWorldPoint(Input.mousePosition));// Is vertex clicked
                                if (a1 != 100)
                                {
                                    focusedObject.GetComponent<Components>().setFocused(true);
                                    focusedWireVertex = a1;
                                    return;
                                }

                                a1 = focusedObject.GetComponent<WireController>().lineClicked(CamMove.cam.ScreenToWorldPoint(Input.mousePosition));// is line clicked
                                if (a1 != 100)
                                {
                                    focusedObject.GetComponent<WireController>().insertVertex(CamMove.cam.ScreenToWorldPoint(Input.mousePosition), a1);
                                    focusedObject.GetComponent<Components>().setFocused(true);
                                    focusedWireVertex = a1;
                                    return;
                                }
                                break;
                            default:

                                break;
                        }
                    }
                    else
                    {
                        if (focusedObject != null)
                        {
                            setFocusedObjectNull();
                        }
                    }
                }
                if (Input.GetMouseButton(0))
                {
                    if (focusedObject != null&&!CircuitAnalyzer.isRunning)
                    {
                        switch (focusedObject.tag)
                        {
                            case "wire":
                                if (isDragComponent) { dragWire(1, focusedWireVertex); }
                                break;
                            default:
                                dragComponents(1, mode, gap);
                                break;
                        }
                    }
                }
                if (Input.GetMouseButtonUp(0))
                {
                    if (focusedObject != null)
                    {
                        switch (focusedObject.tag)
                        {
                            case "wire":
                                hit = Physics2D.Raycast(focusedObject.GetComponent<WireController>().vertexsBox[focusedWireVertex], Vector2.zero, 0f, 1 << 3);
                                if (focusedObject.GetComponent<WireController>().vertexsBox[focusedWireVertex].x % 1 != 0 || focusedObject.GetComponent<WireController>().vertexsBox[focusedWireVertex].y % 1 != 0 ||
                                    focusedObject.GetComponent<WireController>().vertexDeleteCheck(focusedWireVertex) || (hit && hit.collider.gameObject.tag == "components"))
                                {
                                    if (focusedObject.GetComponent<WireController>().vertexsBox.Count < 3) { Debug.Log("ssd"); destroyFocuedObject(); focusedWireVertex = -1; isDragComponent = false; return; }
                                    else { focusedObject.GetComponent<WireController>().removeVertex(focusedWireVertex); }
                                    //wire.GetComponent<LineRenderer>().SetPosition(1, new Vector2(Mathf.RoundToInt(cam.ScreenToWorldPoint(Input.mousePosition).x), Mathf.RoundToInt(cam.ScreenToWorldPoint(Input.mousePosition).y)));
                                }
                                else
                                {
                                    int a = linkWireCheck(focusedWireVertex);
                                    if (a != 100)
                                    {
                                        if (focusedWireVertex == 0 || focusedWireVertex == focusedObject.GetComponent<WireController>().vertexsBox.Count - 1)
                                        {
                                            //Debug.Log(a);
                                            hit.collider.transform.parent.gameObject.GetComponent<WireController>().insertVertex(focusedObject.GetComponent<WireController>().vertexsBox[focusedWireVertex], a);
                                        }
                                        else { focusedObject.GetComponent<WireController>().removeVertex(focusedWireVertex); }
                                    }
                                }
                                focusedObject.GetComponent<WireController>().setCollider();
                                //focusedObject.GetComponent<Components>().resetLinkedObject();
                                focusedWireVertex = -1;
                                isDragComponent = false;
                                break;
                            default:
                                dragComponents(2, mode, gap);
                                break;
                        }
                    }
                }
            }
        }
    }
    void LDrag()
    {

    }
    void RDrag()
    {
        CamMove.cam.GetComponent<CamMove>().camDrag(Input.GetMouseButton(1));
    }
    void dragWire(int state, int i = 0)
    {
        switch (state)
        {
            case 0://down
                if (magnetic(0.3f, 0.7f, Vector2.zero))
                {
                    if (focusedObject != null) { focusedObject.GetComponent<Components>().setFocused(false); setInfo(false); }
                    focusedObject = Instantiate(components[0], null);
                    focusedObject.transform.parent = componentManager.gameObject.transform;
                    focusedObject.GetComponent<Components>().init();
                    focusedObject.GetComponent<WireController>().addWire();
                    focusedObject.GetComponent<Components>().setFocused(true);
                    setInfo(true);
                    Vector2 pos = new Vector2(Mathf.RoundToInt(CamMove.cam.ScreenToWorldPoint(Input.mousePosition).x), Mathf.RoundToInt(CamMove.cam.ScreenToWorldPoint(Input.mousePosition).y));
                    focusedObject.GetComponent<WireController>().addVertex(pos);
                    focusedObject.GetComponent<WireController>().addVertex(pos);
                    componentManager.compo.Add(focusedObject.GetComponent<Components>());
                    int a = linkWireCheck();
                    if (a != 100) { hit.collider.transform.parent.gameObject.GetComponent<WireController>().insertVertex(focusedObject.GetComponent<WireController>().vertexsBox[1], a); }
                }
                else { if (focusedObject != null) { focusedObject.GetComponent<Components>().setFocused(false); setInfo(false); focusedObject = null; } }
                break;
            case 1://drag
                Vector2 pos2 = new Vector2(CamMove.cam.ScreenToWorldPoint(Input.mousePosition).x, CamMove.cam.ScreenToWorldPoint(Input.mousePosition).y);
                if (magnetic(0.3f, 0.7f, Vector2.zero))
                {
                    pos2 = new Vector2(Mathf.RoundToInt(CamMove.cam.ScreenToWorldPoint(Input.mousePosition).x), Mathf.RoundToInt(CamMove.cam.ScreenToWorldPoint(Input.mousePosition).y));
                }
                focusedObject.GetComponent<WireController>().moveVertex(pos2, i);
                /*Vector2 pos = new Vector2(Mathf.RoundToInt(cam.ScreenToWorldPoint(Input.mousePosition).x),Mathf.RoundToInt(cam.ScreenToWorldPoint(Input.mousePosition).y));
                                        wire.GetComponent<LineRenderer>().SetPosition(1, pos);*/
                break;
            case 2://up
                hit = Physics2D.Raycast(focusedObject.GetComponent<WireController>().vertexsBox[1], Vector2.zero, 0f, 1 << 3);
                if (focusedObject.GetComponent<WireController>().vertexsBox[1].x % 1 != 0 || focusedObject.GetComponent<WireController>().vertexsBox[1].y % 1 != 0 ||
                                    focusedObject.GetComponent<WireController>().vertexsBox[1] == focusedObject.GetComponent<WireController>().vertexsBox[0] || (hit && hit.collider.gameObject.tag == "components"))
                {
                    destroyFocuedObject(); Debug.Log("ass");
                }
                else
                {
                    int a = linkWireCheck();
                    if (a != 100) { hit.collider.transform.parent.gameObject.GetComponent<WireController>().insertVertex(focusedObject.GetComponent<WireController>().vertexsBox[1], a); }
                    focusedObject.GetComponent<WireController>().setCollider();
                    //focusedObject.GetComponent<Components>().resetLinkedObject();
                    //componentManager.setLoop(focusedObject.GetComponent<Components>());
                }
                break;
        }

    }
    void dragComponents(int state, int i, Vector2 gap)
    {
        switch (state)
        {
            case 0://down
                if (magnetic(0.3f, 0.7f, gap))
                {
                    if (focusedObject != null) { setFocusedObjectNull(); }
                    focusedObject = Instantiate(components[i - 1], null);
                    focusedObject.transform.parent = componentManager.gameObject.transform;
                    Vector2 pos = new Vector2(Mathf.RoundToInt(CamMove.cam.ScreenToWorldPoint(Input.mousePosition).x - gap.x), Mathf.RoundToInt(CamMove.cam.ScreenToWorldPoint(Input.mousePosition).y - gap.y));
                    focusedObject.transform.position = pos;
                    componentManager.compo.Add(focusedObject.GetComponent<Components>());
                    focusedObject.GetComponent<Components>().init();
                    focusedObject.GetComponent<Components>().setFocused(true);
                    setGraph(focusedObject);
                    //setNameForNewObject(focusedObject);
                    //if (objectOverlapCheck()) { destroyFocuedObject(); return;}
                    setInfo(true);
                }
                else { if (focusedObject != null) { setFocusedObjectNull(); } }
                break;
            case 1://drag
                Vector2 pos2 = new Vector2(CamMove.cam.ScreenToWorldPoint(Input.mousePosition).x - gap.x, CamMove.cam.ScreenToWorldPoint(Input.mousePosition).y - gap.y);
                if (magnetic(0.3f, 0.7f, gap))
                {
                    pos2 = new Vector2(Mathf.RoundToInt(CamMove.cam.ScreenToWorldPoint(Input.mousePosition).x - gap.x), Mathf.RoundToInt(CamMove.cam.ScreenToWorldPoint(Input.mousePosition).y - gap.y));
                }
                focusedObject.transform.position = pos2;
                break;
            case 2://up
                if((focusedObject.transform.position.x % 1) >= 0.9999f) { focusedObject.transform.position = new Vector3(Mathf.Round(focusedObject.transform.position.x), focusedObject.transform.position.y, focusedObject.transform.position.z); }
                if((focusedObject.transform.position.y % 1) >= 0.9999f) { focusedObject.transform.position = new Vector3(focusedObject.transform.position.x, Mathf.Round(focusedObject.transform.position.y), focusedObject.transform.position.z); }
                if ((focusedObject.transform.position.x % 1) != 0  || 
                    (focusedObject.transform.position.y % 1) != 0  ||
                    objectOverlapCheck())
                {
                    Debug.Log("x 0:  " + (focusedObject.transform.position.x) + "   y 0:   " + (focusedObject.transform.position.y));
                    Debug.Log("x:  " + (focusedObject.transform.position.x%1) + "   y:   " + (focusedObject.transform.position.y%1));
                    destroyFocuedObject(); 
                }
                else
                {
                    focusedObject.GetComponent<Components>().setPos(new Vector2(focusedObject.transform.position.x, focusedObject.transform.position.y));
                    linkWireCheckForObject();
                    //focusedObject.GetComponent<Components>().resetLinkedObject();
                    //componentManager.setLoop(focusedObject.GetComponent<Components>());
                }
                break;
        }
    }
    public void setMode(int i)
    {
        if (focusedObject != null) { setFocusedObjectNull();  }
        if (mode == i)
        {
            isDraw = false;
            selectedMenuToggle.isOn = false;
            selectedMenuToggle = null;
            mode = 0;
        }
        else
        {
            if (selectedMenuToggle != null)
            {
                selectedMenuToggle.isOn = false;
            }
            isDraw = true;
            mode = i;
        }

    }
    void setFocusedObjectNull()
    {
        focusedObject.GetComponent<Components>().setFocused(false);
        setInfo(false);
        focusedObject = null;
        graph.reset();
    }
    public void setToggle(Toggle toggle)
    {
        if (isDraw) { selectedMenuToggle = toggle; }
    }
    public void setInfo(bool a)
    {
        switch (focusedObject.name)
        {
            case "WC(Clone)":
                info[0].SetActive(a);
                break;
            case "voltage source(Clone)":
                info[1].SetActive(a);
                focusedObject.GetComponent<Voltage>().info[0] = info[1].transform.GetChild(1).GetComponentInChildren<TMP_InputField>();
                break;
            case "current source(Clone)":
                info[2].SetActive(a);
                focusedObject.GetComponent<Current>().info = info[2].transform.GetChild(1).GetComponentInChildren<TMP_InputField>();
                break;
            case "AC voltage source(Clone)":
                info[3].SetActive(a);
                focusedObject.GetComponent<Voltage>().info[0] = info[3].transform.GetChild(1).GetComponentInChildren<TMP_InputField>();
                focusedObject.GetComponent<Voltage>().info[1] = info[3].transform.GetChild(2).GetComponentInChildren<TMP_InputField>();
                focusedObject.GetComponent<Voltage>().info[2] = info[3].transform.GetChild(3).GetComponentInChildren<TMP_InputField>();
                focusedObject.GetComponent<Voltage>().volText= info[3].transform.GetChild(7).GetChild(0).GetComponentInChildren<TextMeshProUGUI>();
                break;
            case "resistor(Clone)":
                info[4].SetActive(a);
                focusedObject.GetComponent<Resistor>().info = info[4].transform.GetChild(2).GetComponentInChildren<TMP_InputField>();
                focusedObject.GetComponent<Resistor>().infoText[0] = info[4].transform.GetChild(1).gameObject.transform.GetChild(0).GetComponentInChildren<TextMeshProUGUI>();
                focusedObject.GetComponent<Resistor>().infoText[1] = info[4].transform.GetChild(3).gameObject.transform.GetChild(0).GetComponentInChildren<TextMeshProUGUI>();
                focusedObject.GetComponent<Resistor>().printZ();
                break;
            case "capacitor(Clone)":
                info[5].SetActive(a);
                focusedObject.GetComponent<Resistor>().info = info[5].transform.GetChild(2).GetComponentInChildren<TMP_InputField>();
                focusedObject.GetComponent<Resistor>().infoText[0] = info[5].transform.GetChild(1).gameObject.transform.GetChild(0).GetComponentInChildren<TextMeshProUGUI>();
                focusedObject.GetComponent<Resistor>().infoText[1] = info[5].transform.GetChild(3).gameObject.transform.GetChild(0).GetComponentInChildren<TextMeshProUGUI>();
                focusedObject.GetComponent<Resistor>().printZ();
                break;
            case "inductor(Clone)":
                info[6].SetActive(a);
                focusedObject.GetComponent<Resistor>().info = info[6].transform.GetChild(2).GetComponentInChildren<TMP_InputField>();
                focusedObject.GetComponent<Resistor>().infoText[0] = info[6].transform.GetChild(1).gameObject.transform.GetChild(0).GetComponentInChildren<TextMeshProUGUI>();
                focusedObject.GetComponent<Resistor>().infoText[1] = info[6].transform.GetChild(3).gameObject.transform.GetChild(0).GetComponentInChildren<TextMeshProUGUI>();
                focusedObject.GetComponent<Resistor>().printZ();
                break;
            case "switch(Clone)":
                info[7].SetActive(a);
                focusedObject.GetComponent<Switch>().toggle = info[7].transform.GetChild(1).GetComponent<Toggle>();
                break;
            case "ground(Clone)":
                info[8].SetActive(a);
                break;
            default:
                break;
        }
        if (a) { focusedObject.GetComponent<Components>().setInfo(1); }
    }
    bool magnetic(float min, float max, Vector2 gap)
    {
        if ((Mathf.Abs((CamMove.cam.ScreenToWorldPoint(Input.mousePosition).x - gap.x) % 1) <= min || Mathf.Abs((CamMove.cam.ScreenToWorldPoint(Input.mousePosition).x - gap.x) % 1) >= max)
                                && (Mathf.Abs((CamMove.cam.ScreenToWorldPoint(Input.mousePosition).y - gap.y) % 1) <= min || Mathf.Abs((CamMove.cam.ScreenToWorldPoint(Input.mousePosition).y - gap.y) % 1) >= max))
        { return true; }
        else { return false; }
    }

    bool objectOverlapCheck()
    {
        if (focusedObject != null) { if (focusedObject.GetComponent<Components>().isCollided) { Debug.Log("1"); return true; } }
        if (isDraw)
        {
            Vector2 pos = new Vector2(Mathf.RoundToInt(CamMove.cam.ScreenToWorldPoint(Input.mousePosition).x), Mathf.RoundToInt(CamMove.cam.ScreenToWorldPoint(Input.mousePosition).y));
            if (componentManager.compo.Count > 0)
            {
                for (int i = 0; i < componentManager.compo.Count; i++)
                {
                    if (!componentManager.compo[i].vertexsBox.Contains(pos))
                    {
                        if (focusedObject != null)
                        {
                            if (focusedObject.transform.position == componentManager.compo[i].transform.position && focusedObject != componentManager.compo[i].gameObject && componentManager.compo[i].gameObject.name != "WC(Clone)") { Debug.Log("2"); return true; }
                        }
                        else
                        {
                            if (pos == (Vector2)componentManager.compo[i].transform.position && componentManager.compo[i].gameObject.name != "WC(Clone)") { Debug.Log("3"); return true; }
                        }
                    }
                }
            }
        }
        return false;
    }

    public void destroyFocuedObject()
    {
        setInfo(false);
        componentManager.compo.Remove(focusedObject.GetComponent<Components>());
        focusedObject.GetComponent<Components>().destroy();
        focusedObject = null;
        graph.reset();
    }

    int linkWireCheck(int i = 1)
    {
        RaycastHit2D[] R1 = new RaycastHit2D[10];
        R1 = Physics2D.RaycastAll(focusedObject.GetComponent<WireController>().vertexsBox[i], Vector2.zero, 0f, 1 << 3);
        for (int k = 0; k < R1.Length; k++)
        {
            if (R1[k] && R1[k].collider.transform.parent.gameObject.tag == "wire" && R1[k].collider.transform.parent.gameObject != focusedObject)
            {
                hit = R1[k];
                int a1 = hit.collider.transform.parent.gameObject.GetComponent<WireController>().vertexClicked(focusedObject.GetComponent<WireController>().vertexsBox[i]);
                if (a1 != 100) { return 100; }
                a1 = hit.collider.transform.parent.gameObject.GetComponent<WireController>().lineClicked(focusedObject.GetComponent<WireController>().vertexsBox[i]);
                if (a1 != 100) { return a1; }
            }
        }
        return 100;
    }
    void linkWireCheckForObject()
    {
        RaycastHit2D[] R1 = new RaycastHit2D[10];
        for (int i = 0; i < focusedObject.GetComponent<Components>().vertexsBox.Count; i++)
        {
            R1 = Physics2D.RaycastAll(focusedObject.GetComponent<Components>().vertexsBox[i], Vector2.zero, 0f, 1 << 3);
            for (int k = 0; k < R1.Length; k++)
            {
                if (R1[k] && R1[k].collider.transform.parent.gameObject.tag == "wire" && R1[k].collider.transform.parent.gameObject != focusedObject)
                {
                    hit = R1[k];
                    int a1 = hit.collider.transform.parent.gameObject.GetComponent<WireController>().lineClicked(focusedObject.GetComponent<Components>().vertexsBox[i]);
                    if (a1 != 100) { hit.collider.transform.parent.gameObject.GetComponent<WireController>().insertVertex(focusedObject.GetComponent<Components>().vertexsBox[i], a1); }
                }
            }
        }

    }

    void setGraph(GameObject x)
    {
        if (graph.targetObject!=x)
        {
            graph.targetObject = x;
            graph.setToggle();
            //graph.setDD();
        }      
    }

    /*void setNameForNewObject(GameObject ob)
    {
        switch (ob.name)
        {
            case "voltage source(Clone)":
                nameOrder[0]++;
                ob.GetComponent<Components>().OBname = "DC Voltage Source " + nameOrder[0];
                break;
            case "current source(Clone)":
                nameOrder[1]++;
                ob.GetComponent<Components>().OBname = "Current Source " + nameOrder[1];
                break;
            case "AC voltage source(Clone)":
                nameOrder[2]++;
                ob.GetComponent<Components>().OBname = "AC Voltage Source " + nameOrder[2];
                break;
            case "resistor(Clone)":
                nameOrder[3]++;
                ob.GetComponent<Components>().OBname = "Resistor " + nameOrder[3];
                break;
            case "capacitor(Clone)":
                nameOrder[4]++;
                ob.GetComponent<Components>().OBname = "Capacitor " + nameOrder[4];
                break;
            case "inductor(Clone)":
                nameOrder[5]++;
                ob.GetComponent<Components>().OBname = "Inductor " + nameOrder[5];
                break;
        }
    }*/
}
