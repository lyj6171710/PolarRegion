using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ThrowActionBase : MonoBehaviour
{
    protected void ReadyBase()
    {
        mHits = GetComponent<HitTryMake>().meObjsInHit;
        mTargets = new Dictionary<Collider2D, FigureProfile>();
    }

    HashSet<Collider2D> mHits;//向外界引用的
    Dictionary<Collider2D, FigureProfile> mTargets;//省性能用的，避免每帧都遍历所有组件
    //时间系统、考虑同基类
    void Update()
    {
        foreach (Collider2D hit in mHits)
        {
            if (!mTargets.ContainsKey(hit))
            {
                GameObject belong = hit.GetComponent<HitRespondPart>().meBelongGbj;
                mTargets.Add(hit, belong.GetComponent<FigureProfile>());
            }

            mTargets[hit].MakeHurt(this);//暂时默认为一直造成伤害，但可能不会如愿而已
        }

        if (TimeUse.It.SuIfJustPastSpecGap(2)) mTargets.Clear();//重置，清理掉那些不再触碰的对象
    }
}
