using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class HitTryMake : MonoBehaviour,IObvableL<HitTryMake>
{
    //������ײ
    //��Ҫ�������ܣ���Ϊ�����໥��ײ��������Ժܶ�

    //�߼�����ײ������ʽ����ײ�˾�����ײ
    //������ֻҪ�໥�Ӵ����ͻ���Ϊ��������ײ����������Ҫ�Լ��ٹ���һ��
    //�����˹۲���ģʽ����ʱ֪ͨ���۲��Լ���HitRespond

    //��������======================================

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

    //��������======================================

    bool mIsReady = false;

    //������ײ��
    public string mLayerSelf;

    //�ٿ��Ǿ�����Ϸ�߼�
    public string[] mTo;//��ײ�����߻�ο�ʹ��

    bool mCanHit;
    bool mDisposable;//�Ƿ���һ���Ե�
    
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

    bool mHaveHit;//�Ƿ��Ѿ�ײ����һ��

    public HashSet<Collider2D> meObjsInHit;
    //�������е�ǰ���Լ��Ӵ��ĶԷ�����
    //���Ӧ���Լ���֪��ʲô���������������������
    static HashSet<Collider2D> mNoCanHit;
    //��¼������֮������ײ��(��������ײ������û�ﵽ��)���������ټ���ˣ�ȫ��ͨ��
    //������ʵ���е����⣬��������ܻ�ܶ࣬������Щ�������ĵ���

    void OnTriggerStay2D(Collider2D collision)
    {//�������֣��մ���������ײ��Ӧ�߽����ˣ���ʱϣ�����������ģ�stay���б�֤
        
        if (mCanHit && mIsReady)
        {
            if (!mHaveHit)//����ͬʱ�������������ͨ������������������
            {
                if (mNoCanHit.Contains(collision)|| meObjsInHit.Contains(collision)) return;
                //�Ѿ���ײ���������ֲ����з�Ӧ�ģ������ٴ�����ײ
                //�ս�����ײ�󣬳������룬���򲻻��ٴ�����ײ

                HitRespondPart respond = collision.gameObject.GetComponent<HitRespondPart>();
                if (respond != null)
                {
                    if (respond.MakeHit(this))
                    {//�ܹ������໥��ײʱ
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
    {//�����߸����Լ��Ѿ��뿪����
     //������ͨ���Լ���һ�ζ�����������ײ�Ĳ��֣���Ӧ���Լ�
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
    //���ղ�֪ͨ���л�׷���Լ��������о�������

    public void NuAcceptObser(IObser<HitTryMake> obser)
    {
        neObsersHave.NuAcceptNew(obser);
    }

}
