using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ExprReferStr : ExpressAble
{
    public string meStr => str;

    public void SuChangeTo(string value)
    {
        if (value != null && str != value)
        {
            str = value;
            WhenStrChange(str);
        }
    }

    //================================

    string str;

    protected abstract void WhenStrChange(string to);

    protected override void MakeReady()
    {
        base.MakeReady();
        str = "";
    }

    
}
