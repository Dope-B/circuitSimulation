using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ground : Components
{
    public override void init()
    {
        base.init();
    }
    public override void setFocused(bool a)
    {
        base.setFocused(a);
    }
    public override void destroy()
    {
        base.destroy();
    }
    public override void setPos(Vector2 pos)
    {
        if (vertexsBox.Count < 1)
        {
            vertexsBox.Add(pos);
        }
        else
        {
            vertexsBox[0] = pos;
        }
    }
}
