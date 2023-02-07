using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ExprImg : ExprReferImg
{
    public Image bind;

    //=============================

    protected override void WhenImgChange(Sprite to)
    {
        bind.sprite = to;
    }

    protected override void MakeReady()
    {
        base.MakeReady();
        NuWhenEnable += () => { WhenImgChange(meImg); };
    }

}
