using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PROrderTest : PROrder
{
    public override EPROrderComp ID => EPROrderComp.test;

    bool finish;

    public override bool IfFinish()
    {
        return finish;
    }

    public override void Run()
    {
        finish = false;
        Debug.Log("running");
        WaitDeal.It.Begin(() =>
        {
            finish = true;
            Debug.Log("finish");
        }, 2f);
    }

    public override object GetSave()
    {
        throw new System.NotImplementedException();
    }

    public override void LoadData(object dataSave)
    {
        throw new System.NotImplementedException();
    }
}
