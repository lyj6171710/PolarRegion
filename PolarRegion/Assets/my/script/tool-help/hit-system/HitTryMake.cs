using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class HitTryMake : MonoBehaviour,IObvableL<HitTryMake>
{
    //对外碰撞
    //需要考虑性能，因为发生相互碰撞的事物，可以很多

    //逻辑上碰撞，非形式上碰撞了就算碰撞
    //形体上只要相互接触，就会认为发生了碰撞，外界可能需要自己再过滤一下
    //采用了观察者模式，适时通知给观察自己的HitRespond

    //接收数据======================================

    public Action<GameObject> meWhenLayerHit;
    public Action<GameObject> meWhenAgainst;

    public bool meCanHit { 
        get { return mCanHit; } 
        set { 
            mCanHit = value;
            if (!mCanHit)
            {
                neObsersHave.NuOnInvalid();
                meObjsInHit.Clear();
            }
        } 
    }

    //====================================

    public void SuAffectCircle(string filter, float radius)
    {
        if (mIsReady)
        {
            Collider2D[] kicks = RangeCatch.SuAffectCircle(filter, transform.position, radius);
            foreach (Collider2D collider in kicks)
            {
                HitRespond hit = collider.gameObject.GetComponent<HitRespond>();
                //hit.meWhenLayerAffect(collider);
            }
        }
    }

    //自用数据======================================

    bool mIsReady = false;

    //自身碰撞层
    public string mLayerSelf;

    //再考虑具体游戏逻辑
    public string[] mTo;//碰撞接收者会参考使用

    bool mCanHit;
    bool mDisposable;//是否是一次性的
    
    public HitTryMake MakeReady(string layerSelf, bool cost = false, bool startHit = true)
    {
        if (!mIsReady)
        {
            mLayerSelf = layerSelf;
            mCanHit = startHit;
            mDisposable = cost ? true : false;

            gameObject.layer = LayerMask.NameToLayer(mLayerSelf);
            meWhenAgainst += (c) => { };
            meWhenLayerHit += (c) => { };
            meObjsInHit = new HashSet<Collider2D>();
            neObsersHave = new BroadcastToObser<HitTryMake>(this);
            if (mNoCanHit == null) mNoCanHit = new HashSet<Collider2D>();

            mHaveHit = false;

        }
        return this;
    }

    public void ReadyAgainst(params string[] aims)
    {
        mTo = aims;

        mIsReady = true;
    }

    bool mHaveHit;//是否已经撞击过一次

    public HashSet<Collider2D> meObjsInHit;
    //包含所有当前与自己接触的对方物体
    //外界应该自己是知道什么样的情况，可以主动利用
    static HashSet<Collider2D> mNoCanHit;
    //记录不会与之发生碰撞的(连基本碰撞条件都没达到的)，就懒得再检查了，全局通用
    //这里其实还有点问题，空物体可能会很多，比如那些被消减的敌人

    void OnTriggerStay2D(Collider2D collision)
    {//经常出现，刚创建就与碰撞响应者交叉了，此时希望是能碰到的，stay更有保证
        
        if (mCanHit && mIsReady)
        {
            if (!mHaveHit)//可能同时触发多个，这里通过标记来避免这种情况
            {
                if (mNoCanHit.Contains(collision)|| meObjsInHit.Contains(collision)) return;
                //已经碰撞过，但发现不会有反应的，不会再触发碰撞
                //刚进入碰撞后，除非相离，否则不会再触发碰撞

                HitRespondPart respond = collision.gameObject.GetComponent<HitRespondPart>();
                if (respond != null)
                {
                    if (respond.MakeHit(this))
                    {//能够产生相互碰撞时
                        meObjsInHit.Add(collision);
                        if (mDisposable)
                            mHaveHit = true;
                    }
                }
                else
                    mNoCanHit.Add(collision);
            }
        }
    }

    public void TellLeaveByRespond(Collider2D partFirst)
    {//接收者告诉自己已经离开了它
     //接收者通过自己第一次对它产生了碰撞的部分，反应给自己
        meObjsInHit.Remove(partFirst);
    }

    private void OnDestroy()
    {
        neObsersHave.NuOnInvalid();
    }

    private void OnDisable()
    {
        neObsersHave.NuOnInvalid();
        meObjsInHit.Clear();
    }

    public BroadcastToObser<HitTryMake> neObsersHave { get; set; }
    //接收并通知所有会追踪自己接下来行径的外者

    public void NuAcceptObser(IObser<HitTryMake> obser)
    {
        neObsersHave.NuAcceptNew(obser);
    }

}
