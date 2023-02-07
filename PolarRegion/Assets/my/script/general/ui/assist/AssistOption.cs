using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AssistOption : MonoBehaviour,AssistClick.ITouch
{
    //��AssistSelectʹ��
    //��Ӧһ����ѡѡ������

    AssistSelect mLeader;
    AssistClick mMouse;
    AssistPos mPos;
    public AssistPos meSitu => mPos;

    int mIndex;
    
    public void MakeReady()
    {
        mPos = GbjAssist.AddCompSafe<AssistPos>(gameObject);
        mPos.MakeReady();
    }

    public void MakeUse(AssistSelect leader, int index)
    {
        mLeader = leader;
        mIndex = index;

        mMouse = gameObject.AddComponent<AssistClick>();
        mMouse.SuReactTouch(this);
    }//�����Ӧ���Ա�����

    public void MakeDown()//������Ҫѡ��ʱ�����״̬���ȴ��ٴα�ʹ��
    {
        mLeader = null;
        mIndex = -1;
        Destroy(mMouse);
        //Destroy(mPos);//λ�ø���û�б�Ҫ�Ƴ���������������������ο����������õ�
    }

    //�������Ч������������Ӱ��========================

    public virtual void FollowEffectHover(bool onoff) { }
    public virtual void FollowNotValid(bool hide) { }
    public virtual void FollowEffectClick() { }

    //==================================================

    protected void ExciteLeaderHover(int part) 
    {
        mLeader.ExciteSelectByOption(mIndex, part);
    }

    protected void ExciteLeaderClick(int part)
    {
        mLeader.ExciteConfirmByOption(mIndex, part);
    }

    public void iHoverInsideFmMouse(object para)
    {
        ExciteLeaderHover(0);
    }

    public void iHoverOutsideFmMouse(object para)
    {
        
    }

    //===========================================


}
