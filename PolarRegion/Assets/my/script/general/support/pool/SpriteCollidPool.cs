using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteCollidPool : GbjPool, ISwitchScene
{
    public List<GameObject> mPreBuildsDp;

    protected override void HowRecovery(GameObject item)
    {
        Component[] comps = item.GetComponents<Component>();
        for (int i = 3; i < comps.Length; i++)
        {//第一个组件是transform，不能销毁
         //第二个sprite，第三个collid
            Destroy(comps[i]);
        }
    }

    //==============================

    public static SpriteCollidPool It;

    public void WhenAwake()
    {
        It = this;
        Initial((item) => {
            item.AddComponent<SpriteRenderer>();
            item.AddComponent<BoxCollider2D>();
        }, mPreBuildsDp);
    }

    public void WhenSwitchScene()
    {

    }
}