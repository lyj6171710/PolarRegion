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
        {//��һ�������transform����������
         //�ڶ���sprite��������collid
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