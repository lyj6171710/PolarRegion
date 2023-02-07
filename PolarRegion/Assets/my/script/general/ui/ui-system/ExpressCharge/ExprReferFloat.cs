using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.CSharp;

public abstract class ExprReferFloat : ExpressAble
{
    public Vector2 rangeFloat;

    public float meFloat => now;

    public void SuChangeTo(float value)
    {
        if (now != value)
        {
            if (value < now)
            {
                if (value >= rangeFloat.x)
                {
                    now = value;
                    WhenFloatChange(now);
                }
            }
            else
            {
                if (value <= rangeFloat.y)
                {
                    now = value;
                    WhenFloatChange(now);
                }
            }
        }
    }

    //===============================

    float now;
    
    protected abstract void WhenFloatChange(float to);

    protected override void MakeReady()
    {
        base.MakeReady();
        now = rangeFloat.x;
    }
}
