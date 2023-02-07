using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ExprReferImg : ExpressAble
{
    public Sprite meImg => img;

    public void SuChangeTo(Sprite value)
    {
        if (img != value && value != null)
        {
            img = value;
            WhenImgChange(img);
        }
    }

    //=================================

    Sprite img;

    protected abstract void WhenImgChange(Sprite to);

    protected override void MakeReady()
    {
        base.MakeReady();
    }
}
