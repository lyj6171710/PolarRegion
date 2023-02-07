using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class HitRespond : MonoBehaviour,IObser<HitTryMake>
{//���ƶ���ײ�Ľ���
 //��Ҫ�������ܣ���Ϊ�����໥��ײ��������Ժܶ�
 
    //��������======================================
    
    public Action<GameObject> meWhenLayerHit;
    public Action<GameObject> meWhenAgainst;

    public bool meCanHit;

    //��������======================================

    bool mIsReady = false;

    //�ȿ��ܲ����໥ײ��
    List<string> mLayerReact;

    //�ٿ��Ǿ�����Ϸ�߼�
    string[] mBelongs;//�Լ��Ĵ���
    bool mAgainstMulti;//������ݶ�������ʱ�Ŵ���

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
                //�����Ӵ�����
                hitBring.meWhenLayerHit(gameObject);
                meWhenLayerHit(touch);

                //��Ϸ���ݷ�Ӧ����
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

                    if (!mRemains.ContainsKey(touch))//�п������������Ӵ�������ײ�壬���������Լ���Ͻ
                    {
                        mRemains.Add(touch, partFrom.neCollid);
                        mInTrigger.Add(touch);//��Ҫ��������ӣ������һ�ν���ֲ�B��Ȼ���뿪�ֲ�A���ᱻ��Ϊ���뿪������
                    }
                    if (!mHaveAgainsts.ContainsKey(touch))
                        mHaveAgainsts.Add(touch, hitBring);//��gameObject��hitMake��Ҫ�ӿ�
                    
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

    public bool SuWhtherInHit(GameObject hitBy)//�Ƿ������Լ��ص�
    {
        if (mRemains.ContainsKey(hitBy))
            return true;
        else
            return false;
    }

    public bool SuHaveHitAgainThreaten(HitTryMake hitMake)
    {
        //�Ƿ����ٴ��ܵ���ײ����в
        //�����ǹ۲���Լ���ɹ���ײ�Ķ����Ƿ��Ѿ�����
        if (mHaveAgainsts.ContainsValue(hitMake))
            return true;
        else
            return false;
    }

    Dictionary<GameObject, Collider2D> mRemains;//���ڽӴ��ģ�gameObject���м����
    Dictionary<GameObject, HitTryMake> mHaveAgainsts;//���Լ��������Ӵ��ģ�ͬʱҲ�𵽼��ٲ�ѯ���ã�
    //Ԫ������Ӧ���ܱ�Remain���������Ӧ�ú�Remain��ӦԪ��һ����ʧ

    List<GameObject> mInTrigger;//����Ԫ���ظ�

    public void OnTriggerIn(Collider2D collision)
    {//�Ը����Լ�һ���ֲ����ߵ�ͬʱ�����뵽�Լ���һ���ֲ����������˲����뿪�Ӵ��Լ�
        GameObject touch = collision.gameObject;
        if (mRemains.ContainsKey(touch))//��Ҫ�Ѿ�����Ϊ�Ӵ��ߵģ�������Ч
        {
            mInTrigger.Add(touch);
        }
    }

    public void OnTriggerOut(Collider2D collision)
    {//����������Ӧ��ײ���뿪��ã���Ϊ�����ߵ��������ڼ���Χ���������ӵ�
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
        //�����ʧЧ�˵�(���ܻ�δ�뿪��ײ�壬���Ի��˵�)
        mHaveAgainsts.Remove(obvable.gameObject);
    }

    public void OnNext(HitTryMake obvable, InfoSeal ifo)
    {
        throw new NotImplementedException();
    }

}
