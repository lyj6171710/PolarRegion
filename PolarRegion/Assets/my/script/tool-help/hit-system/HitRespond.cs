using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class HitRespond : MonoBehaviour,IObser<HitTryMake>
{//控制对碰撞的接收
 //需要考虑性能，因为发生相互碰撞的事物，可以很多
 
    //接收数据======================================
    
    public Action<GameObject> meWhenLayerHit;
    public Action<GameObject> meWhenAgainst;

    public bool meCanHit;

    //自用数据======================================

    bool mIsReady = false;

    //先看能不能相互撞击
    List<string> mLayerReact;

    //再考虑具体游戏逻辑
    string[] mBelongs;//自己的从属
    bool mAgainstMulti;//所有身份都被包含时才触发

    public HitRespond MakeReady(params string[] resist)
    {
        if (!mIsReady)
        {
            meWhenAgainst += (c) => { };
            meWhenLayerHit += (c) => { };
            mHaveAgainsts = new Dictionary<GameObject, HitTryMake>();
            mRemains = new Dictionary<GameObject, Collider2D>();
            mInTrigger = new List<GameObject>();
            neReadyStopObserToAll = new ReadyStopObserToAll();

            ReadyHitReact(resist);

            meCanHit = true;
        }
        return this;
    }

    public void ReadyBelong(bool multi, params string[] mySides)
    {
        mBelongs = mySides;

        mAgainstMulti = multi;

        mIsReady = true;
    }

    public bool MakeHit(HitTryMake hitBring, HitRespondPart partFrom)
    {
        if (meCanHit && mIsReady)
        {
            //if (gameObject.name == "pawn1")
            //mBelongs[0].ff();
            GameObject touch = hitBring.gameObject;

            if (mRemains.ContainsKey(touch)) return false;

            if (StrAnalyse.HaveSame(mLayerReact.ToArray(), LayerMask.LayerToName(touch.layer)))
            {
                //场景接触层面
                hitBring.meWhenLayerHit(gameObject);
                meWhenLayerHit(touch);

                //游戏内容反应层面
                bool touchAgainst = false;

                if (mAgainstMulti)
                {
                    if (StrAnalyse.IsSupport(hitBring.mTo, mBelongs)) touchAgainst = true;
                }
                else
                {
                    if (StrAnalyse.HaveSame(hitBring.mTo, mBelongs)) touchAgainst = true;
                }
                
                if (touchAgainst)
                {
                    meWhenAgainst(touch);
                    hitBring.meWhenAgainst(gameObject);

                    if (!mRemains.ContainsKey(touch))//有可能外物连续接触两个碰撞体，但都属于自己管辖
                    {
                        mRemains.Add(touch, partFrom.neCollid);
                        mInTrigger.Add(touch);//需要在这里添加，否则第一次进入局部B，然后离开局部A，会被认为是离开了整体
                    }
                    if (!mHaveAgainsts.ContainsKey(touch))
                        mHaveAgainsts.Add(touch, hitBring);//从gameObject到hitMake需要加快
                    
                    hitBring.NuAcceptObser(this);

                    return true;
                }
            }
        }
        return false;
    }

    void ReadyHitReact(params string[] resist)
    {
        if (mLayerReact == null) mLayerReact = new List<string>();
        foreach (string one in resist)
        {
            if (!mLayerReact.Contains(one))
                mLayerReact.Add(one);
        }
    }

    //====================================

    public bool SuWhtherInHit(GameObject hitBy)//是否还在与自己重叠
    {
        if (mRemains.ContainsKey(hitBy))
            return true;
        else
            return false;
    }

    public bool SuHaveHitAgainThreaten(HitTryMake hitMake)
    {
        //是否还有再次受到碰撞的威胁
        //本意是观察对自己造成过碰撞的东西是否已经消匿
        if (mHaveAgainsts.ContainsValue(hitMake))
            return true;
        else
            return false;
    }

    Dictionary<GameObject, Collider2D> mRemains;//正在接触的，gameObject是中间介质
    Dictionary<GameObject, HitTryMake> mHaveAgainsts;//与自己发生过接触的（同时也起到加速查询作用）
    //元素周期应该总比Remain长，否则就应该和Remain对应元素一起消失

    List<GameObject> mInTrigger;//允许元素重复

    public void OnTriggerIn(Collider2D collision)
    {//对付从自己一个局部出走的同时，进入到自己另一个局部的情况，因此不算离开接触自己
        GameObject touch = collision.gameObject;
        if (mRemains.ContainsKey(touch))//需要已经被列为接触者的，才能有效
        {
            mInTrigger.Add(touch);
        }
    }

    public void OnTriggerOut(Collider2D collision)
    {//接收者来感应碰撞者离开最好，因为接收者的生命周期及范围往往高于子弹
        GameObject touch = collision.gameObject;
        if (mRemains.ContainsKey(touch))
        {
            mInTrigger.SelfRemoveFirst(touch);
            if (!mInTrigger.Contains(touch))
            {
                mHaveAgainsts[touch].TellLeaveByRespond(mRemains[touch]);
                mRemains.Remove(touch);
            }
        }
    }

    void OnDestroy()
    {
        neReadyStopObserToAll.NuMake();
    }

    public ReadyStopObserToAll neReadyStopObserToAll { get; set; }

    public void OnInvalid(HitTryMake obvable)
    {
        mRemains.Remove(obvable.gameObject);
        //清理掉失效了的(可能还未离开碰撞体，就自毁了的)
        mHaveAgainsts.Remove(obvable.gameObject);
    }

    public void OnNext(HitTryMake obvable, InfoSeal ifo)
    {
        throw new NotImplementedException();
    }

}
