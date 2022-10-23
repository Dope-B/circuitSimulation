using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MaterialController : MonoBehaviour
{
    [Range(4,100)]
    public float tiling=4;
    [Range(0f,1f)]
    public float alpha;
    public Color gridColor;
    public Color boxColor;
    [Range(0f,1f)]
    public float lineThickness;
    private void Start()
    {
       
    }
    private void Update()
    {
        setAlpha();
        setColor();
        setlineThickness();
        setTiling();
        setBoxColor();
    }
    void setTiling()
    {
        GetComponent<Renderer>().material.SetFloat("_tiling", tiling);     
    }
    void setAlpha()
    {
        GetComponent<Renderer>().material.SetFloat("_alpha", alpha);   
    }
    void setColor()
    {
        GetComponent<Renderer>().material.SetColor("_gridColor", gridColor);
    }
    void setlineThickness()
    {
        GetComponent<Renderer>().material.SetFloat("_lineThickness", lineThickness);  
    }
    void setBoxColor()
    {
        GetComponent<Renderer>().material.SetColor("_boxColor", boxColor); 
    }

}
