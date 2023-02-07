using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ExprReferInt : ExpressAble
{
    public Vector2Int rangeInt;

    public int meInt => num;

    public void SuChangeTo(int value)
    {
        if (num != value)
        {
            if (value < num)
            {
                if (value >= rangeInt.x)
                {
                    num = value;
                    WhenIntChange(num);
                }
            }
            else
            {
                if (value <= rangeInt.y)
                {
                    num = value;
                    WhenIntChange(num);
                }
            }
        }
    }

    //=================================

    int num;
    
    protected abstract void WhenIntChange(int num);

    protected override void MakeReady()
    {
        base.MakeReady();
        num = rangeInt.x;
    }
}
