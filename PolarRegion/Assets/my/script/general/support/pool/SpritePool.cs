using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpritePool : GbjPool, ISwitchScene
{

    protected override void HowRecovery(GameObject item)
    {
        Component[] comps = item.GetComponents<Component>();
        for (int i = 2; i < comps.Length; i++)
        {//��һ�������transform����������
         //�ڶ�������϶���spriteRender��Ҳ��������
            Destroy(comps[i]);
            //if (!TypeAssist.WhetherTypeIs<SpriteRenderer>(comps[i]))
            //    Destroy(comps[i]);
        }
    }

    //==============================

    public static SpritePool It;

    public void WhenAwake()
    {
        It = this;
        Initial((item) => { item.AddComponent<SpriteRenderer>(); });
    }

    public void WhenSwitchScene()
    {
        
    }
}
