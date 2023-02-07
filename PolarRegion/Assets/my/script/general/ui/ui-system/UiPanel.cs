using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EStateSubIn { Use, Down, Open, Close }

public class UiPanel : MonoBehaviour
{
    //�����ǣ�������Ƕ����壬ÿ�����Ψһӵ�����ɸ��ض���ѡ��

    //��壬֧�����ϻ��ݣ�֧��ͬ���л���֧��Ƕ���ڽ�
    //������ֱ�ӹ�Ͻ��ѡ��֧�ֿ���
    //�������֧�ֵĽ�������Ϊ������ֱϽѡ����������
    //���Ľ������������Ͻ��ѡ����䣬������û��ѡ����Ҳ��û�пɽ�����Ԫ��
    //������Ϊ��һ����������ɸ�ѡ����ɣ�Ȼ��������ϡ����¡��������ҿ����������

    //���ձ���ʱ��������Լ�����������
    //��屻��ʱ����������Ϊ����ѡ��Ҳ��������Ϊ���Ҫ��
    //�������Ϊѡ����ô��ѡ��Ҫ�������������������Ϊ��磬��ô��Ĭ��ֵ

    string division;

    //==============================

    public bool meInUse => mInUse;
    public bool meInDown => gameObject.activeSelf && !mInUse;
    public bool meInOpen => gameObject.activeSelf;
    
    [HideInInspector]public bool mAsMainPanel;//����߲㼶����壬���������κ����

    bool meHaveList => mListMager != null;//����ֱϽѡ��
    bool meHaveSubInOpen => mCountSubInOpen != 0;

    [HideInInspector]public UiPanel mSuper;
    internal UiListsCtrl mListMager;//��������ֱϽѡ��ѡ��û��Ƕ����
    bool mInUse;
    bool mHaveReady;
    int mCountSubInOpen;

    void MakeReady()
    {
        ReadySub();
        mHaveReady = true;
    }//û�б��õ�ʱ�����������׼��(�������������ٶ�)

    void MakeUse()
    {
        if (meHaveList) mListMager.FollowUse();
        mInUse = true;
    }//ȷ����崦��open״̬�󣬲��ܵ���ִ�и÷���
    //����忪��ʱ���丸�����һ��Ҳ�账�ڿ���״̬������������������

    void MakeDown()
    {
        //һ�������Ӽ���ͣ��
        mSubPanels.ForEachCanModify((sub)=> SureGetDown(sub));
        if (meHaveList) mListMager.FollowDown();
        mInUse = false;
    }

    //---------------------------------

    internal void MakeCome(UiPanel which)
    {
        if (mAsMainPanel || (which == mSuper && mSuper.meInOpen))
        { //��������壬ֻ֧���ɸ�����ʹ�ã�����������Ҳ��Ҫ�������ڿ���״̬
            if (!mHaveReady) MakeReady();
            if (!meInOpen) gameObject.SetActive(true);
            if (!mInUse) MakeUse();
        }
    }
    //�Ƿ����ȡ�����û���Ҫ��ѡ����Ҫ
    //ѡ������������κε�ǰ����״̬����µ���һ���
    //ѡ��������ǰ����ѡ���ǿ���״̬��
    //ѡ����״̬������������ǿ���״̬��
    //����ǿ���״̬��ǰ�����丸������ǿ���״̬
    //�û�������������ǰ��Ҳ��ͬ��ģ�����״̬��ѡ�����ܱ��û����������

    internal void MakeLeave()
    {
        if (mInUse) MakeDown();
        if (meInOpen) gameObject.SetActive(false);
    }

    //===================================

    Dictionary<UiPanel, EStateSubIn> mSubPanels;

    internal void TakeLeaveToSuper()
    {
        MakeLeave();
        if (!mSuper.meHaveSubInOpen)
            mListMager.FollowContinue();//ѡ����Ϊ����
    }

    internal void TakeFoward(UiPanel sub)
    {
        if (SureGetUse(sub))
            mListMager.FollowPause();//ѡ����Ϊ��ͣ
    }

    internal void TakeSideComeInUp(UiPanel partner)
    {
        UiPanel partnerSuper = Method.ReverseFindInRecur(this,
            (one) => one.mSuper,//������Ϊ����һ�����ڿ���״̬
            (one) => one.NuEnumSubPanelInOpen(),//�������е�ǰ��״̬�����
            (one) =>
            {
                if (one.mSubPanels.ContainsKey(partner))
                {
                    if (one.mSubPanels[partner] != EStateSubIn.Use)
                        return true;//�ҵ���
                }
                return false;
            });
            partnerSuper.TakeFoward(partner);
    }

    //================================

    IEnumerable<UiPanel> NuEnumSubPanelInOpen()
    {
        var iter = mSubPanels.Keys.GetEnumerator();
        while (iter.MoveNext())
        {
            if (!iter.Current.meInOpen)
                yield return iter.Current;
        }
        yield break;
    }

    bool SureGetUse(UiPanel sub)
    {
        if (mSubPanels[sub] != EStateSubIn.Use)
        {
            if (!sub.meInOpen) mCountSubInOpen++;
            sub.MakeCome(this);
            mSubPanels[sub] = EStateSubIn.Use;
            return true;
        }
        else return false;
    }

    bool SureGetDown(UiPanel sub)
    {
        if (mSubPanels[sub] == EStateSubIn.Use)
        {
            sub.MakeDown();
            mSubPanels[sub] = EStateSubIn.Down;
            return true;
        }
        else return false;
    }

    bool SureGetClose(UiPanel sub)
    {
        if (mSubPanels[sub] != EStateSubIn.Close)
        {
            sub.MakeLeave();
            mSubPanels[sub] = EStateSubIn.Close;
            mCountSubInOpen--;
            return true;
        }
        else return false;
    }

    void ReadySub()
    {
        mListMager = GetComponent<UiListsCtrl>();
        mSubPanels = new Dictionary<UiPanel, EStateSubIn>();
        for (int i = 0; i < transform.childCount; i++)
        {
            Transform child = transform.GetChild(i);
            UiPanel panel = child.GetComponent<UiPanel>();
            if (panel != null)
            {
                panel.mSuper = this;
                panel.gameObject.SetActive(false);//����Ǳ�Ҫ�ģ����Ҳ�ʹ��leave
                mSubPanels.Add(panel, EStateSubIn.Close);
            }
            else
            {//˳���¼�����е�list
                UiList list = child.GetComponent<UiList>();
                if (list != null)
                {
                    list.mSuper = this;
                    mListMager.FollowAcceptOneSubList(list);
                }
            }
        }
    }

    //======================================

    internal bool SuWhetherHave(UiPanel sub)
    {
        return mSubPanels.ContainsKey(sub);
    }

}

