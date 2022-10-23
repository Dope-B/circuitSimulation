using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TabSliding : MonoBehaviour
{
    bool isOn = false;
    bool isCwork = false;
    public int offX;
    public int offY;
    public int onX;
    public int onY;
    RectTransform rect;
    // Start is called before the first frame update
    void Start()
    {
        rect = GetComponent<RectTransform>();
    }

    // Update is called once per frame
    void Update()
    {

    }
    public void scrolling(int type)
    {
        if (!isCwork)
        {
            switch (type)
            {
                case 1: StartCoroutine(Cscrolling01()); break;
                case 2: StartCoroutine(Cscrolling02()); break;
                default:
                    break;
            }
        }
        
    }
    IEnumerator Cscrolling01()
    {
        isCwork = true;
        if (isOn)
        {
            Vector3 v1 = new Vector3(offX, offY);
            while (Vector3.Distance(rect.anchoredPosition,v1)>0.3f)
            {
                rect.anchoredPosition = Vector3.Lerp(rect.anchoredPosition, new Vector2(offX, offY), 20 * Time.deltaTime);
                yield return new WaitForEndOfFrame();
            }

            rect.anchoredPosition = new Vector2(offX, offY);
            isOn = false;
        }
        else
        {
            Vector3 v1 = new Vector3(onX, onY);
            while (Vector3.Distance(rect.anchoredPosition, v1) > 0.3f)
            {
                rect.anchoredPosition = Vector3.Lerp(rect.anchoredPosition, new Vector2(onX, onY), 20 * Time.deltaTime);
                yield return new WaitForEndOfFrame();
            }

            rect.anchoredPosition = new Vector2(onX, onY);
            isOn = true;
        }
        isCwork = false;
        yield return null;
    }
    IEnumerator Cscrolling02()
    {
        isCwork = true;
        Vector2 v1 = Vector3.Lerp(new Vector2(onX, onY), new Vector2(offX, offY), 0.2f);
        if (isOn)
        {
            
            for (int i = 0; i < 5; i++)
            {
                rect.anchoredPosition += v1;
                yield return new WaitForEndOfFrame();
            }
            isOn = false;
            rect.anchoredPosition = new Vector2(offX, offY);
        }
        else
        {
            for (int i = 0; i < 5; i++)
            {
                rect.anchoredPosition -= v1;
                yield return new WaitForEndOfFrame();
            }
            isOn = true;
            rect.anchoredPosition = new Vector2(onX, onY);
        }
        isCwork = false;
        yield return null;
    }
}
