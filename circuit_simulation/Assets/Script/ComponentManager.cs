using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ComponentManager : MonoBehaviour
{
    public GameObject nodeFlag;
    List<GameObject> flagBox;
    public TextMeshProUGUI timeText;// use on CircuitAnalyzer
    [System.Serializable]
    public class Loop
    {
        public List<Components> components;
        public float resistance;
        public List<Node> nodes;
        public float reactance;
        public List<int> groundIndex;
        public bool isDirect = true;// Alternative or Direct
    }
    public List<Components> compo;
    public List<Loop> loops;
    [System.Serializable]
    public class Node
    {
        public float voltage;
        public Vector2 pos;
        public List<LNO> LNO;
        public List<Components> linkedObject;
        public List<int> linkedNodeIndex;
    }
    
    [System.Serializable]
    public struct LNO
    {
        public List<Components> linkedObject;
        public int linkedNodeIndex;
    }
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    public void setLoop()
    {
        /*if (loops.Count != 0)
        {
            for (int i = 0; i < loops.Count; i++)
            {
                for (int j = 0; j < loops[i].components.Count; j++)
                {
                    if (loops[i].components.Contains(g) && !loops[i].components[j].linkedObject.Contains(g)) { loops[i].components.Remove(g); return; }
                    else if (!loops[i].components.Contains(g) && loops[i].components[j].linkedObject.Contains(g)) { loops[i].components.Add(g); return; }
                }
            }
        }
        addLoop(g);*/
        if (compo.Count == 0) { return; }
        for (int i = 0; i < compo.Count; i++)
        {
            compo[i].resetLinkedObject();
            compo[i].resetData();
        }
        loops = new List<Loop>();
        addLoop(0);
        for (int i = 0; i < compo.Count; i++)
        {
            bool b1 = false;
            for (int x = 0; x < loops.Count; x++)
            {
                if (loops[x].components.Contains(compo[i])) { b1 = false; break; }
                b1 = true;
            }
            if (b1) { addLoop(i); }
        }
        cleanUpLink();
        setNode();
        setComponentVertexNode();
    }
    void addLoop(int i)
    {
        /*Loops loopBox = new Loops();
        loopBox.components = new List<Components>();
        loopBox.nodes = new List<Node>();
        loopBox.components.Add(g);
        loops.Add(loopBox);*/
        Loop loopBox = new Loop();
        loopBox.components = new List<Components>();
        loopBox.groundIndex = new List<int>();
        loopBox.components.Add(compo[i]);
        for (int j = 0; j < loopBox.components.Count; j++)
        {
            for (int k = 0; k < loopBox.components[j].linkedObject.Count; k++)
            {
                if (!loopBox.components.Contains(loopBox.components[j].linkedObject[k]))
                {
                    loopBox.components.Add(loopBox.components[j].linkedObject[k]);
                }
            }
        }
        if(loopBox.components.Find(x=>x.gameObject.name=="AC voltage source(Clone)")) { loopBox.isDirect = false; }
        loops.Add(loopBox);
    }
    void setNode()
    {
        for (int i = 0; i < loops.Count; i++)
        {
            loops[i].nodes = new List<Node>();
            for (int j = 0; j < loops[i].components.Count; j++)
            {
                if (loops[i].components[j].gameObject.name != "WC(Clone)" &&
                    loops[i].components[j].gameObject.name != "ground(Clone)")
                {
                    Node NBox = new Node();
                    NBox.linkedObject = new List<Components>();
                    for (int k = 0; k < 2; k++)
                    {
                        bool a = false;
                        List<Components> CBox = new List<Components>();
                        CBox.Add(loops[i].components[j]);
                        CBox.AddRange(loops[i].components[j].Vertexs[k].linkedObject);
                        CBox = wireNodeCheck(CBox);
                        for (int g = 0; g < loops[i].nodes.Count; g++)
                        {
                            if (!canMakeNodeCheck(loops[i].components[j], k, loops[i].nodes[g].pos)) { a = true; break; }
                        }
                        if (!a)
                        {
                            NBox.pos = loops[i].components[j].Vertexs[k].vertex;
                            NBox.linkedObject.AddRange(CBox);
                            NBox.linkedNodeIndex = new List<int>();
                            NBox.LNO = new List<LNO>();
                            NBox.voltage = 0;
                            if (NBox.linkedObject.Count >= 2) { loops[i].nodes.Add(NBox); }
                            NBox = new Node();
                            NBox.linkedObject = new List<Components>();
                            
                        }
                    }
                }
            }
        }
        setLinkedNode();
        setLNO();
    }
    void setFlag()
    {
        flagBox = new List<GameObject>();
        for (int i = 0; i < loops.Count; i++)
        {
            for (int j = 0; j < loops[i].nodes.Count; j++)
            {
                GameObject nf = Instantiate(nodeFlag);
                nf.transform.position = loops[i].nodes[j].pos;
                nf.transform.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text = "v" + j;
                flagBox.Add(nf);
            }
        }
    }
    void deleteFlag()
    {
        for (int i = 0; i < flagBox.Count; i++)
        {
            Destroy(flagBox[i]);
        }
        flagBox.Clear();
    }
    void setLinkedNode()
    {
        for (int i = 0; i < loops.Count; i++)
        {
            for (int j = 0; j < loops[i].nodes.Count; j++)
            {
                for (int k = 0; k < loops[i].nodes.Count; k++)
                {
                    if (j != k)
                    {
                        for (int x = 0; x < loops[i].nodes[k].linkedObject.Count; x++)
                        {
                            if (loops[i].nodes[j].linkedObject.Contains(loops[i].nodes[k].linkedObject[x]))
                            {
                                if (!loops[i].nodes[j].linkedNodeIndex.Contains(k)) { loops[i].nodes[j].linkedNodeIndex.Add(k); }
                            }
                        }
                    }
                }
            }
        }
    }
    public List<Components> wireNodeCheck(List<Components> g)
    {

        List<Components> box = new List<Components>();
        box.AddRange(g);
        for (int i = 0; i < box.Count; i++)
        {
            if (box[i].gameObject.name == "WC(Clone)")
            {
                for (int j = 0; j < box[i].linkedObject.Count; j++)
                {
                    if (!box.Contains(box[i].linkedObject[j])) { box.Add(box[i].linkedObject[j]); }
                }
            }
        }
        for (int i = 0; i < box.Count; i++)
        {
            if (box[i].gameObject.name == "WC(Clone)") { box.Remove(box[i]); i--; }
        }
        return box;
    }
    bool ignoreObjectCheck(Components g)
    {
        if (g.gameObject.name == "ground(Clone)" || g.gameObject.name == "switch(Clone)") { return true; }
        else { return false; }
    }

    void setLNO()
    {
        for (int i = 0; i < loops.Count; i++)
        {
            for (int j = 0; j < loops[i].nodes.Count; j++)
            {

                for (int k = 0; k < loops[i].nodes[j].linkedNodeIndex.Count; k++)
                {
                    LNO box = new LNO();
                    box.linkedNodeIndex = new int();
                    box.linkedObject = new List<Components>();
                    for (int x = 0; x < loops[i].nodes[loops[i].nodes[j].linkedNodeIndex[k]].linkedObject.Count; x++)
                    {
                        if (loops[i].nodes[j].linkedObject.Contains(loops[i].nodes[loops[i].nodes[j].linkedNodeIndex[k]].linkedObject[x])) { box.linkedObject.Add(loops[i].nodes[loops[i].nodes[j].linkedNodeIndex[k]].linkedObject[x]); }
                    }
                    box.linkedNodeIndex = loops[i].nodes[j].linkedNodeIndex[k];
                    loops[i].nodes[j].LNO.Add(box);
                }
            }
        }
    }

    void cleanUpLink()
    {
        for (int i = 0; i < loops.Count; i++)
        {
            for (int j = 0; j < loops[i].components.Count; j++)
            {
                switch (loops[i].components[j].gameObject.name)
                {
                    case "switch(Clone)":// on-> open
                        for (int k = 0; k < 2; k++)
                        {
                            for (int z = 0; z < loops[i].components[j].Vertexs[k].linkedObject.Count; z++)
                            {
                                for (int x = 0; x < loops[i].components[j].Vertexs[k].linkedObject[z].Vertexs.Count; x++)
                                {
                                    if (loops[i].components[j].Vertexs[k].linkedObject[z].Vertexs[x].linkedObject.Contains(loops[i].components[j]))
                                    {
                                        if (loops[i].components[j].GetComponent<Switch>().isOn)
                                        {
                                            if (k == 0)
                                            {
                                                for (int c = 0; c < loops[i].components[j].Vertexs[1].linkedObject.Count; c++)
                                                {
                                                    if (!loops[i].components[j].Vertexs[k].linkedObject[z].Vertexs[x].linkedObject.Contains(loops[i].components[j].Vertexs[1].linkedObject[c]))
                                                    {
                                                        loops[i].components[j].Vertexs[k].linkedObject[z].Vertexs[x].linkedObject.Add(loops[i].components[j].Vertexs[1].linkedObject[c]);
                                                    }
                                                    if (!loops[i].components[j].Vertexs[k].linkedObject[z].linkedObject.Contains(loops[i].components[j].Vertexs[1].linkedObject[c]))
                                                    {
                                                        loops[i].components[j].Vertexs[k].linkedObject[z].linkedObject.Add(loops[i].components[j].Vertexs[1].linkedObject[c]);
                                                    }
                                                }

                                            }
                                            else
                                            {
                                                for (int c = 0; c < loops[i].components[j].Vertexs[0].linkedObject.Count; c++)
                                                {
                                                    if (!loops[i].components[j].Vertexs[k].linkedObject[z].Vertexs[x].linkedObject.Contains(loops[i].components[j].Vertexs[0].linkedObject[c]))
                                                    {
                                                        loops[i].components[j].Vertexs[k].linkedObject[z].Vertexs[x].linkedObject.Add(loops[i].components[j].Vertexs[0].linkedObject[c]);
                                                    }
                                                    if (!loops[i].components[j].Vertexs[k].linkedObject[z].linkedObject.Contains(loops[i].components[j].Vertexs[0].linkedObject[c]))
                                                    {
                                                        loops[i].components[j].Vertexs[k].linkedObject[z].linkedObject.Add(loops[i].components[j].Vertexs[0].linkedObject[c]);
                                                    }
                                                }
                                            }
                                        }
                                        loops[i].components[j].Vertexs[k].linkedObject[z].Vertexs[x].linkedObject.Remove(loops[i].components[j]);
                                        loops[i].components[j].Vertexs[k].linkedObject[z].linkedObject.Remove(loops[i].components[j]);
                                    }
                                }
                            }

                        }
                        for (int k = 0; k < 2; k++)
                        {
                            loops[i].components[j].Vertexs[k].linkedObject.Clear();
                        }
                        loops[i].components[j].linkedObject.Clear();
                        break;
                    case "inductor(Clone)":
                        if (loops[i].isDirect)
                        {
                            for (int k = 0; k < loops[i].components[j].Vertexs.Count; k++)
                            {
                                for (int z = 0; z < loops[i].components[j].Vertexs[k].linkedObject.Count; z++)
                                {
                                    for (int x = 0; x < loops[i].components[j].Vertexs[k].linkedObject[z].Vertexs.Count; x++)
                                    {
                                        if (loops[i].components[j].Vertexs[k].linkedObject[z].Vertexs[x].linkedObject.Contains(loops[i].components[j]))
                                        {
                                            if (k == 0)
                                            {
                                                for (int c = 0; c < loops[i].components[j].Vertexs[1].linkedObject.Count; c++)
                                                {
                                                    if (!loops[i].components[j].Vertexs[k].linkedObject[z].Vertexs[x].linkedObject.Contains(loops[i].components[j].Vertexs[1].linkedObject[c]))
                                                    {
                                                        loops[i].components[j].Vertexs[k].linkedObject[z].Vertexs[x].linkedObject.Add(loops[i].components[j].Vertexs[1].linkedObject[c]);
                                                    }
                                                    if (!loops[i].components[j].Vertexs[k].linkedObject[z].linkedObject.Contains(loops[i].components[j].Vertexs[1].linkedObject[c]))
                                                    {
                                                        loops[i].components[j].Vertexs[k].linkedObject[z].linkedObject.Add(loops[i].components[j].Vertexs[1].linkedObject[c]);
                                                    }
                                                }

                                            }
                                            else
                                            {
                                                for (int c = 0; c < loops[i].components[j].Vertexs[0].linkedObject.Count; c++)
                                                {
                                                    if (!loops[i].components[j].Vertexs[k].linkedObject[z].Vertexs[x].linkedObject.Contains(loops[i].components[j].Vertexs[0].linkedObject[c]))
                                                    {
                                                        loops[i].components[j].Vertexs[k].linkedObject[z].Vertexs[x].linkedObject.Add(loops[i].components[j].Vertexs[0].linkedObject[c]);
                                                    }
                                                    if (!loops[i].components[j].Vertexs[k].linkedObject[z].linkedObject.Contains(loops[i].components[j].Vertexs[0].linkedObject[c]))
                                                    {
                                                        loops[i].components[j].Vertexs[k].linkedObject[z].linkedObject.Add(loops[i].components[j].Vertexs[0].linkedObject[c]);
                                                    }
                                                }
                                            }
                                            loops[i].components[j].Vertexs[k].linkedObject[z].Vertexs[x].linkedObject.Remove(loops[i].components[j]);
                                            loops[i].components[j].Vertexs[k].linkedObject[z].linkedObject.Remove(loops[i].components[j]);
                                        }
                                    }
                                }
                            }
                            for (int k = 0; k < loops[i].components[j].Vertexs.Count; k++)
                            {
                                loops[i].components[j].Vertexs[k].linkedObject.Clear();
                            }
                            loops[i].components[j].linkedObject.Clear();
                        }
                        break;
                    case "capacitor(Clone)":
                        if (loops[i].isDirect)
                        {
                            for (int k = 0; k < 2; k++)
                            {
                                for (int z = 0; z < loops[i].components[j].Vertexs[k].linkedObject.Count; z++)
                                {
                                    for (int x = 0; x < loops[i].components[j].Vertexs[k].linkedObject[z].Vertexs.Count; x++)
                                    {
                                        if (loops[i].components[j].Vertexs[k].linkedObject[z].Vertexs[x].linkedObject.Contains(loops[i].components[j]))
                                        {
                                            loops[i].components[j].Vertexs[k].linkedObject[z].Vertexs[x].linkedObject.Remove(loops[i].components[j]);
                                            loops[i].components[j].Vertexs[k].linkedObject[z].linkedObject.Remove(loops[i].components[j]);
                                        }
                                    }
                                }
                            }
                            for (int k = 0; k < 2; k++)
                            {
                                loops[i].components[j].Vertexs[k].linkedObject.Clear();
                            }
                            loops[i].components[j].linkedObject.Clear();
                        }
                        break;
                }
            }
        }
    }

    int tragVertex(Components target,Components tracker,int startPoint)
    {
        List<Components> CL=new List<Components>();
        CL.AddRange(tracker.Vertexs[startPoint].linkedObject);
        for (int i = 0; i < CL.Count; i++)
        {
            if (CL[i].gameObject.name == "WC(Clone)")
            {
                for (int j = 0; j < CL[i].linkedObject.Count; j++)
                {
                    if (!CL.Contains(CL[i].linkedObject[j])) { CL.Add(CL[i].linkedObject[j]); }
                }
            }
        }
        for (int i = 0; i < CL.Count; i++)
        {
            if (CL[i].gameObject.name != "WC(Clone)") { CL.Remove(CL[i]);i--; }
        }
        for (int k = 0; k < target.Vertexs.Count; k++)
        {
            for (int i = 0; i < CL.Count; i++)
            {
                for (int j = 0; j < CL[i].Vertexs.Count; j++)
                {
                    if (CL[i].Vertexs[j].vertex == target.Vertexs[k].vertex) { return k; }
                }
            }
        }
        return 100;
    }

    bool canMakeNodeCheck(Components tracker, int startPoint,Vector2 nodePos)
    {
        List<Components> CL = new List<Components>();
        CL.AddRange(tracker.Vertexs[startPoint].linkedObject);
        for (int i = 0; i < CL.Count; i++)
        {
            if (CL[i].gameObject.name == "WC(Clone)")
            {
                for (int j = 0; j < CL[i].linkedObject.Count; j++)
                {
                    if (!CL.Contains(CL[i].linkedObject[j])) { CL.Add(CL[i].linkedObject[j]); }
                }
            }
        }
        for (int i = 0; i < CL.Count; i++)
        {
            if (CL[i].gameObject.name != "WC(Clone)") { CL.Remove(CL[i]); i--; }
        }
        if (CL.Count == 0)
        {
            if (tracker.Vertexs[startPoint].vertex == nodePos) { return false; }
        }
        else
        {
            for (int i = 0; i < CL.Count; i++)
            {
                for (int j = 0; j < CL[i].Vertexs.Count; j++)
                {
                    if (nodePos == CL[i].Vertexs[j].vertex) { return false; }// do not make node
                }
            }
        }
        return true;// make node
    }
    public int findDirection(Components target, Vector2 nodePos)
    {
        /*for (int i = 0; i < 2; i++)                                           method 1
        {
            if (target.Vertexs[i].vertex == nodePos) { return i; }
        }*/
        for (int i = 0; i < 2; i++)
        {
            List<Components> CL = new List<Components>();
            CL.AddRange(target.Vertexs[i].linkedObject);
            for (int j = 0; j < CL.Count; j++)
            {
                if (CL[j].gameObject.name == "WC(Clone)")
                {
                    for (int k = 0; k < CL[j].linkedObject.Count; k++)
                    {
                        if (!CL.Contains(CL[j].linkedObject[k])) { CL.Add(CL[j].linkedObject[k]); }
                    }
                }
            }
            for (int j = 0; j < CL.Count; j++)
            {
                if (CL[j].gameObject.name == "WC(Clone)") { CL.Remove(CL[j]); j--; }
            }
            for (int j = 0; j < CL.Count; j++)
            {
                for (int k = 0; k < CL[j].Vertexs.Count; k++)
                {
                    if (CL[j] == target && CL[j].Vertexs[k].vertex == nodePos) { return k; }// method 2
                    else if (CL[j] != target && CL[j].Vertexs[k].vertex == nodePos) { return i; }
                }
            }
        }
        return 100;
    }

    void setComponentVertexNode()
    {
        for (int i = 0; i < loops.Count; i++)
        {
            for (int j = 0; j < loops[i].nodes.Count; j++)
            {
                for (int k = 0; k < loops[i].nodes[j].linkedObject.Count; k++)
                {
                    //int vt = findDirection(loops[i].nodes[j].linkedObject[k], loops[i].nodes[j].pos);
                    if (canMakeNodeCheck(loops[i].nodes[j].linkedObject[k],0, loops[i].nodes[j].pos)) { loops[i].nodes[j].linkedObject[k].Vertexs[0].linkedNode = loops[i].nodes[j]; }
                    else if (canMakeNodeCheck(loops[i].nodes[j].linkedObject[k], 1, loops[i].nodes[j].pos)) { loops[i].nodes[j].linkedObject[k].Vertexs[1].linkedNode = loops[i].nodes[j]; }
                }
            }
        }
    }
    
}
