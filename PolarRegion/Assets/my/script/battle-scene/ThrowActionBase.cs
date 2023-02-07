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

    HashSet<Collider2D> mHits;//��������õ�
    Dictionary<Collider2D, FigureProfile> mTargets;//ʡ�����õģ�����ÿ֡�������������
    //ʱ��ϵͳ������ͬ����
    void Update()
    {
        foreach (Collider2D hit in mHits)
        {
            if (!mTargets.ContainsKey(hit))
            {
                GameObject belong = hit.GetComponent<HitRespondPart>().meBelongGbj;
                mTargets.Add(hit, belong.GetComponent<FigureProfile>());
            }

            mTargets[hit].MakeHurt(this);//��ʱĬ��Ϊһֱ����˺��������ܲ�����Ը����
        }

        if (TimeUse.It.SuIfJustPastSpecGap(2)) mTargets.Clear();//���ã��������Щ���ٴ����Ķ���
    }
}
