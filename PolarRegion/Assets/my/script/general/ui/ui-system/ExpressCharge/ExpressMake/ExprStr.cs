using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ExprStr : ExprReferStr
{
    public Text to;

    //=============================

    protected override void WhenStrChange(string state)
    {
        to.text = state;
    }

    protected override void MakeReady()
    {
        base.MakeReady();
        NuWhenEnable += () => { WhenStrChange(meStr); };
    }
}
