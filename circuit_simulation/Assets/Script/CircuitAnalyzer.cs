using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;
public class CircuitAnalyzer : MonoBehaviour
{
    public ComponentManager CM;
    public WaitForSecondsRealtime WFS;
    public MouseControl MC;
    bool isErrorOccurred;
    float time = 0f;
    float term;
    float frame;
    public static bool isRunning = false;

    // Start is called before the first frame update
    void Start()
    {
        term = 0.01f;
        WFS = new WaitForSecondsRealtime(0.01f);
    }

    // Update is called once per frame
    void Update()
    {

    }
    public void run() { isRunning = true; StartCoroutine("start"); }
    void setTerm(float x)
    {
        if (term / x >= 10)
        {
            CM.timeText.color = Color.black;
            StopCoroutine("twinkleTimeText");
            StartCoroutine("twinkleTimeText");
            int a = (int)(term / x) / 10;
            int b = 1;
            while (a >= 10)
            {
                a = (int)(a / 10);
                b += 1;
            }
            term = term / Mathf.Pow(10, b);
        }
        else if (term / x < 1f)
        {
            term = 0.01f;
            if (term / x <= 10)
            {
                CM.timeText.color = Color.black;
                StopCoroutine("twinkleTimeText");
                StartCoroutine("twinkleTimeText");
            }
            setTerm(x);

        }
    }
    IEnumerator start()
    {
        CM.timeText.text = time.ToString("F4");
        setMatrix();
        setgraphVertex();
        yield return WFS;
        time += term;
        StartCoroutine("start");
    }
    public void stop()
    {
        StopCoroutine("start");
        isRunning = false;
        term = 0.01f;
        time = 0;
        resetLC();
        CM.timeText.text = time.ToString("F5");
        MC.graph.resetYPos();
        MC.graph.resetScaleY();
        resetText();
    }

    void setMatrix()
    {
        isErrorOccurred = false;
        setGround();
        checkVoltage();
        if (isErrorOccurred) { return; }
        for (int i = 0; i < CM.loops.Count; i++)
        {
            List<float[]> matrix = new List<float[]>();
            List<float> matrix2 = new List<float>();
            for (int j = 0; j < CM.loops[i].nodes.Count; j++)
            {
                float[] MBox = new float[CM.loops[i].nodes.Count];
                float result = 0;
                for (int k = 0; k < CM.loops[i].nodes[j].LNO.Count; k++)
                {
                    int vt;// linkedVertex 1--> + 0--> -
                    for (int p = 0; p < CM.loops[i].nodes[j].LNO[k].linkedObject.Count; p++)
                    {
                        switch (CM.loops[i].nodes[j].LNO[k].linkedObject[p].gameObject.name)
                        {
                            case "voltage source(Clone)":
                                MBox = new float[CM.loops[i].nodes.Count];
                                vt = CM.findDirection(CM.loops[i].nodes[j].LNO[k].linkedObject[p], CM.loops[i].nodes[j].pos);
                                if (vt == 1)
                                {
                                    MBox[j] = 1;
                                    MBox[CM.loops[i].nodes[j].LNO[k].linkedNodeIndex] = -1;
                                }
                                else if (vt == 0)
                                {
                                    MBox[j] = -1;
                                    MBox[CM.loops[i].nodes[j].LNO[k].linkedNodeIndex] = 1;
                                }
                                result = CM.loops[i].nodes[j].LNO[k].linkedObject[p].GetComponent<Voltage>().getVoltage();
                                p = CM.loops[i].nodes[j].LNO[k].linkedObject.Count - 1;// to break;
                                k = CM.loops[i].nodes[j].LNO.Count - 1;
                                break;
                            case "current source(Clone)":
                                vt = CM.findDirection(CM.loops[i].nodes[j].LNO[k].linkedObject[p], CM.loops[i].nodes[j].pos);
                                if (vt == 1)
                                {
                                    result += CM.loops[i].nodes[j].LNO[k].linkedObject[p].GetComponent<Current>().getCurrent();
                                }
                                else if (vt == 0)
                                {
                                    result -= CM.loops[i].nodes[j].LNO[k].linkedObject[p].GetComponent<Current>().getCurrent();
                                }
                                break;
                            case "AC voltage source(Clone)":
                                setTerm(1 / (CM.loops[i].nodes[j].LNO[k].linkedObject[p].GetComponent<Voltage>().getFrequency() * 100));
                                MBox = new float[CM.loops[i].nodes.Count];
                                vt = CM.findDirection(CM.loops[i].nodes[j].LNO[k].linkedObject[p], CM.loops[i].nodes[j].pos);
                                if (vt == 1)
                                {
                                    MBox[j] = 1;
                                    MBox[CM.loops[i].nodes[j].LNO[k].linkedNodeIndex] = -1;
                                }
                                else if (vt == 0)
                                {
                                    MBox[j] = -1;
                                    MBox[CM.loops[i].nodes[j].LNO[k].linkedNodeIndex] = 1;
                                }
                                result = CM.loops[i].nodes[j].LNO[k].linkedObject[p].GetComponent<Voltage>().getVoltage(time);
                                CM.loops[i].nodes[j].LNO[k].linkedObject[p].GetComponent<Voltage>().volText.text = result.ToString("F4");
                                p = CM.loops[i].nodes[j].LNO[k].linkedObject.Count - 1;// to break;
                                k = CM.loops[i].nodes[j].LNO.Count - 1;
                                break;
                            case "resistor(Clone)":
                                MBox[j] += 1 / CM.loops[i].nodes[j].LNO[k].linkedObject[p].GetComponent<Resistor>().getZ();
                                MBox[CM.loops[i].nodes[j].LNO[k].linkedNodeIndex] -= 1 / CM.loops[i].nodes[j].LNO[k].linkedObject[p].GetComponent<Resistor>().getZ();
                                break;
                            case "capacitor(Clone)":
                                float x1 = CM.loops[i].nodes[j].LNO[k].linkedObject[p].GetComponent<Resistor>().getZ() / term;
                                if (!CM.loops[i].isDirect)
                                {
                                    MBox[j] += x1;
                                    MBox[CM.loops[i].nodes[j].LNO[k].linkedNodeIndex] -= x1;
                                    result += x1 * CM.loops[i].nodes[j].LNO[k].linkedObject[p].GetComponent<Resistor>().previousVoltage;
                                }
                                else { }
                                break;
                            case "inductor(Clone)":
                                float x2 = term / CM.loops[i].nodes[j].LNO[k].linkedObject[p].GetComponent<Resistor>().getZ();
                                if (!CM.loops[i].isDirect)
                                {
                                    MBox[j] += x2;
                                    MBox[CM.loops[i].nodes[j].LNO[k].linkedNodeIndex] -= x2;
                                    result -= x2 * CM.loops[i].nodes[j].LNO[k].linkedObject[p].GetComponent<Resistor>().intergral;
                                }
                                else { }
                                break;
                            default:
                                break;
                        }
                    }
                }
                if (matrix.Find(x => x.SequenceEqual(MBox)) == null)
                {
                    matrix.Add(MBox);
                    matrix2.Add(result);
                }
            }
            applyGround(matrix, matrix2, CM.loops[i].groundIndex);
            //printMatrix(matrix, matrix2, CM.loops[i].groundIndex);
            List<float> voltage = new List<float>();
            voltage.AddRange(getData(matrix, matrix2, getDet(matrix)));
            for (int j = 0; j < CM.loops[i].nodes.Count; j++)
            {
                if (!CM.loops[i].groundIndex.Contains(j))
                {
                    CM.loops[i].nodes[j].voltage = voltage[0];
                    voltage.RemoveAt(0);
                }
            }
            if (isErrorOccurred) { return; }
        }
        setData();
    }
    void setGround()
    {
        for (int i = 0; i < CM.loops.Count; i++)
        {
            bool a = false;// is ground exist
            for (int j = 0; j < CM.loops[i].nodes.Count; j++)
            {
                for (int k = 0; k < CM.loops[i].nodes[j].linkedObject.Count; k++)
                {
                    if (CM.loops[i].nodes[j].linkedObject[k].gameObject.name == "ground(Clone)" && !CM.loops[i].groundIndex.Contains(j))
                    {
                        a = true;
                        CM.loops[i].groundIndex.Add(j);
                    }
                }
            }
            if (!a)
            {
                int groundIndex = 0;
                for (int j = 0; j < CM.loops[i].nodes.Count - 1; j++)
                {
                    if (CM.loops[i].nodes[j].linkedObject.Count >= CM.loops[i].nodes[j + 1].linkedObject.Count) { groundIndex = j; }
                }
                if (!CM.loops[i].groundIndex.Contains(groundIndex)) { CM.loops[i].groundIndex.Add(groundIndex); }

                return;
            }
        }
    }

    void setData()
    {
        for (int i = 0; i < CM.loops.Count; i++)
        {
            for (int j = 0; j < CM.loops[i].components.Count; j++)
            {
                if (CM.loops[i].components[j].Vertexs[0].linkedNode != null && CM.loops[i].components[j].Vertexs[1].linkedNode != null)
                {
                    switch (CM.loops[i].components[j].gameObject.name)
                    {
                        case "resistor(Clone)":
                            CM.loops[i].components[j].GetComponent<Resistor>().voltage = float.Parse((CM.loops[i].components[j].Vertexs[0].linkedNode.voltage - CM.loops[i].components[j].Vertexs[1].linkedNode.voltage).ToString("F4"));
                            CM.loops[i].components[j].GetComponent<Resistor>().current = float.Parse(((CM.loops[i].components[j].Vertexs[0].linkedNode.voltage - CM.loops[i].components[j].Vertexs[1].linkedNode.voltage) / CM.loops[i].components[j].GetComponent<Resistor>().getZ()).ToString("F4"));
                            break;
                        case "inductor(Clone)":
                            CM.loops[i].components[j].GetComponent<Resistor>().voltage = float.Parse((CM.loops[i].components[j].Vertexs[0].linkedNode.voltage - CM.loops[i].components[j].Vertexs[1].linkedNode.voltage).ToString("F4"));
                            CM.loops[i].components[j].GetComponent<Resistor>().intergral += term * CM.loops[i].components[j].GetComponent<Resistor>().voltage;
                            float a = CM.loops[i].components[j].GetComponent<Resistor>().intergral / CM.loops[i].components[j].GetComponent<Resistor>().getZ();
                            CM.loops[i].components[j].GetComponent<Resistor>().current = float.Parse(a.ToString("F4"));
                            break;
                        case "capacitor(Clone)":
                            CM.loops[i].components[j].GetComponent<Resistor>().previousVoltage = CM.loops[i].components[j].GetComponent<Resistor>().voltage;
                            CM.loops[i].components[j].GetComponent<Resistor>().voltage = float.Parse((CM.loops[i].components[j].Vertexs[0].linkedNode.voltage - CM.loops[i].components[j].Vertexs[1].linkedNode.voltage).ToString("F4"));
                            float b = (CM.loops[i].components[j].GetComponent<Resistor>().getZ() * (CM.loops[i].components[j].GetComponent<Resistor>().voltage - CM.loops[i].components[j].GetComponent<Resistor>().previousVoltage)) / term;
                            CM.loops[i].components[j].GetComponent<Resistor>().current = float.Parse(b.ToString("F4"));
                            break;
                    }
                }


            }

        }
        if (MC.focusedObject != null) { MC.focusedObject.GetComponent<Components>().setInfo(); }

    }

    void applyGround(List<float[]> matrix, List<float> matrix2, List<int> groundIndex)
    {
        for (int i = 0; i < matrix.Count; i++)// delete ground voltage each matrix row
        {
            List<float> box = new List<float>();
            for (int j = 0; j < matrix[i].Length; j++)
            {
                if (!groundIndex.Contains(j)) { box.Add(matrix[i][j]); }
            }

            if (box.Count == 0)
            {
                matrix.RemoveAt(i);
                matrix2.RemoveAt(i);
            }
            else
            {
                matrix[i] = new float[box.Count];
                for (int j = 0; j < matrix[i].Length; j++)
                {
                    matrix[i][j] = box[j];
                }
            }
        }
        if (matrix.Count > 0 && matrix[0].Length != matrix.Count)
        {
            List<float[]> matrixBox = new List<float[]>();
            List<float> matrix2Box = new List<float>();
            for (int j = 0; j < matrix.Count; j++)
            {
                if (!groundIndex.Contains(j)) { matrixBox.Add(matrix[j]); matrix2Box.Add(matrix2[j]); }
            }
            matrix.Clear();
            matrix2.Clear();
            matrix.AddRange(matrixBox);
            matrix2.AddRange(matrix2Box);
        }
    }
    void checkVoltage()
    {
        for (int i = 0; i < CM.loops.Count; i++)
        {
            for (int j = 0; j < CM.loops[i].nodes.Count; j++)
            {
                for (int k = 0; k < CM.loops[i].nodes[j].LNO.Count; k++)
                {
                    bool isVolExist = false;
                    for (int g = 0; g < CM.loops[i].nodes[j].LNO[k].linkedObject.Count; g++)
                    {
                        if (CM.loops[i].nodes[j].LNO[k].linkedObject[g].gameObject.name == "voltage source(Clone)" || CM.loops[i].nodes[j].LNO[k].linkedObject[g].gameObject.name == "AC voltage source(Clone)")
                        {
                            if (!isVolExist) { isVolExist = true; }
                            else { isErrorOccurred = true; }
                        }
                    }
                }
            }
        }
    }
    void checkCurrent()
    {
        for (int i = 0; i < CM.loops.Count; i++)
        {
            for (int j = 0; j < CM.loops[i].nodes.Count; j++)
            {
                bool isCurExist = false;
                for (int k = 0; k < CM.loops[i].nodes[j].LNO.Count; k++)
                {
                    for (int g = 0; g < CM.loops[i].nodes[j].LNO[k].linkedObject.Count; g++)
                    {
                        if (CM.loops[i].nodes[j].LNO[k].linkedObject[g].gameObject.name == "current source(Clone)")
                        {
                            if (!isCurExist) { isCurExist = true; }
                            else
                            {
                                if (CM.loops[i].nodes[j].LNO[k].linkedObject.Count != 1)
                                {
                                    for (int x = 0; x < CM.loops[i].nodes[j].LNO[k].linkedObject.Count; x++)
                                    {
                                        if (CM.loops[i].nodes[j].LNO[k].linkedObject[x].gameObject.name != "current source(Clone)") { return; }
                                    }
                                    isErrorOccurred = true;
                                }
                            }
                        }
                    }
                }
            }
        }
    }// do not use
    /*IEnumerator getMatrixDet(List<float[]> matrix)
    {
        float[] box = new float[matrix[0].Length];
        for (int i = 0; i < matrix[0].Length; i++)
        {
            List<float[]> matBox = new List<float[]>();
            matBox.AddRange(matrix);
            matBox.Remove(matBox[0]);
            for (int j = 0; j < matBox.Count; j++)
            {
                List<float> x = new List<float>();
                for (int k = 0; k < matBox[j].Length; k++)
                {
                    if (k != i) { x.Add(matBox[j][k]); }
                }
                matBox[i] = new float[matBox[i].Length - 1];
                for (int k = 0; k < matBox[i].Length; k++)
                {
                    matBox[j][k] = x[i];
                }
            }
            //box[i] = matBox[0][i] * getDetSub(matBox);
            if (matBox[0].Length > 2) {  StartCoroutine(getMatrixDet(matBox)); }
            else { box[i] = (matBox[0][0] * matBox[1][0]) - (matBox[0][1] * matBox[1][0]); }
        }
        float det = 0;
        for (int i = 0; i < box.Length; i++)
        {

            if (i % 2 == 0) { det -= matrix[0][i] * box[i]; }
            else { det += matrix[0][i] * box[i]; }
        }
        yield return det;
    }*/

    float getDet(List<float[]> matrix)
    {
        float det = 0;
        if (matrix.Count > 2)
        {
            float[] box = new float[matrix[0].Length];
            for (int i = 0; i < matrix[0].Length; i++)
            {
                List<float[]> matBox = new List<float[]>();
                matBox.AddRange(matrix);
                matBox.Remove(matBox[0]); // delete row
                for (int j = 0; j < matBox.Count; j++) // delete col
                {
                    List<float> x = new List<float>();
                    for (int k = 0; k < matBox[j].Length; k++)
                    {
                        if (k != i) { x.Add(matBox[j][k]); }
                    }
                    matBox[j] = new float[matrix[j].Length - 1];
                    for (int k = 0; k < matBox[j].Length; k++)
                    {
                        matBox[j][k] = x[k];
                    }
                }
                //box[i] = matBox[0][i] * getDetSub(matBox);
                if (matBox[0].Length > 2) { box[i] = getDet(matBox); }// get subMatrix det
                else { Debug.Log(matBox[0].Length+"  "+matBox.Count); box[i] = (matBox[0][0] * matBox[1][1]) - (matBox[0][1] * matBox[1][0]); }
            }
            for (int i = 0; i < box.Length; i++)
            {
                det += matrix[0][i] * box[i] * Mathf.Pow(-1, i);// get main Matrix det
            }
        }
        else if (matrix.Count == 2) { det = (matrix[0][0] * matrix[1][1]) - (matrix[0][1] * matrix[1][0]); }
        else if (matrix.Count == 1) { det = matrix[0][0]; }
        return det;
    }

    List<float> getData(List<float[]> matrix, List<float> array, float det)
    {
        if (det == 0) { Debug.Log("Error"); isErrorOccurred = true; return new List<float>(); }
        //Debug.Log("det:  " + det);
        float[,] inverseMatrix = new float[matrix.Count, matrix.Count];
        List<float> voltage = new List<float>();
        if (matrix.Count == 1) { array[0] = array[0] / matrix[0][0]; return array; }
        for (int i = 0; i < matrix.Count; i++)
        {
            for (int j = 0; j < matrix[0].Length; j++)// j-> delete col num
            {
                List<float[]> matBox = new List<float[]>();
                matBox.AddRange(matrix);
                matBox.Remove(matBox[i]); // delete row
                for (int g = 0; g < matBox.Count; g++)
                {
                    List<float> x = new List<float>();// make box for new row
                    for (int k = 0; k < matBox[g].Length; k++)
                    {
                        if (k != j) { x.Add(matBox[g][k]); }
                    }
                    matBox[g] = new float[matBox[g].Length - 1];
                    for (int k = 0; k < matBox[g].Length; k++) // add new row
                    {
                        matBox[g][k] = x[k];
                    }
                }
                inverseMatrix[j, i] = getDet(matBox) * Mathf.Pow(-1, i + j);
            }
        }
        //Debug.Log("--------inverseMatrix------------");
        for (int i = 0; i < inverseMatrix.GetLength(0); i++)
        {
            float voltageBox = 0;
            //string x = null;
            for (int j = 0; j < array.Count; j++)
            {
                voltageBox += inverseMatrix[i, j] * array[j];
                //x += inverseMatrix[i, j] + "   ";
                //if (j == array.Count - 1) { x += array[i]; }
            }
            //Debug.Log(x);
            voltage.Add(voltageBox * (1 / det));

        }
        /*Debug.Log("--------------voltage--------------");
        for (int i = 0; i < voltage.Count; i++)
        {
            Debug.Log("v" + i + "=   " + voltage[i]);
        }*/
        return voltage;
    }

    void printMatrix(List<float[]> matrix, List<float> matrix2, List<int> groundIndex)
    {
        for (int i = 0; i < matrix.Count; i++)
        {
            string x = null;
            int num = 0;
            for (int j = 0; j < matrix[i].Length; j++)
            {
                if (groundIndex.Contains(j))
                {
                    num++; x += "    " + matrix[i][j] + " * v" + num;
                }
                else
                {
                    x += "    " + matrix[i][j] + " * v" + num; num++;
                }

            }
            x += "  =  ";
            x += matrix2[i];
            Debug.Log(x);
        }
        
    }

    public void onOffButton(Button button)
    {
        if (isRunning) { button.interactable = false; }
        else { button.interactable = true; }
    }
    public void offButton(Button button)
    {
        if (isRunning) { button.interactable = true; }
        else { button.interactable = false; }
    }
    void setgraphVertex()
    {
        if (MC.graph.targetObject != null)
        {
            switch (MC.graph.targetObject.gameObject.name)
            {
                case "resistor(Clone)":
                case "inductor(Clone)":
                case "capacitor(Clone)":
                    MC.graph.scalingY(Mathf.Max(Mathf.Abs(MC.graph.targetObject.GetComponent<Resistor>().voltage), Mathf.Abs(MC.graph.targetObject.GetComponent<Resistor>().current)));
                    MC.graph.setGraphYPos(MC.graph.targetObject.GetComponent<Resistor>().voltage, 0);
                    MC.graph.setGraphYPos(MC.graph.targetObject.GetComponent<Resistor>().current, 1);
                    break;
                case "voltage source(Clone)":
                case "AC voltage source(Clone)":
                    MC.graph.scalingY(MC.graph.targetObject.GetComponent<Voltage>().getMaxVoltage());
                    MC.graph.setGraphYPos(MC.graph.targetObject.GetComponent<Voltage>().getVoltage(time));
                    break;
                case "current source(Clone)":
                    MC.graph.scalingY(MC.graph.targetObject.GetComponent<Current>().getCurrent());
                    MC.graph.setGraphYPos(MC.graph.targetObject.GetComponent<Current>().getCurrent());
                    break;

            }
        }
        else
        {

        }
    }

    IEnumerator twinkleTimeText()
    {
        Color defaultColor = Color.black;
        for (float i = 0; i < 1; i += 0.1f)
        {
            CM.timeText.color = Color.Lerp(defaultColor, Color.red, i);
            yield return new WaitForSeconds(0.02f);
        }
        CM.timeText.color = new Color(255, 100, 100);
        for (float i = 0; i < 1; i += 0.1f)
        {
            CM.timeText.color = Color.Lerp(Color.red, defaultColor, i);
            yield return new WaitForSeconds(0.02f);
        }
        CM.timeText.color = defaultColor;
    }

    void resetLC()
    {
        for (int i = 0; i < CM.compo.Count; i++)
        {
            if (CM.compo[i].gameObject.name == "inductor(Clone)") { CM.compo[i].GetComponent<Resistor>().intergral = 0; }
            else if(CM.compo[i].gameObject.name == "capacitor(Clone)") { CM.compo[i].GetComponent<Resistor>().previousVoltage = 0; }
        }
    }

    void resetText()
    {
        for (int i = 0; i < CM.compo.Count; i++)
        {
            CM.compo[i].GetComponent<Components>().resetText();
        }
    }
}
