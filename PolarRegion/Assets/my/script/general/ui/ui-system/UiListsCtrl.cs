using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class UiListsCtrl : MonoBehaviour
{
    //����Ҳ��UiPanel�Ĺ��ܣ������������Ū����

    //һ������¿���ͬʱ���ڶ��ѡ�����ҿ��Էֱ��������ѡ��

    //һ����������崦�ڿ���״̬ʱ��������ֱ����������ѡ�����ܱ�����
    //��Ҫ������Ҳ���ǲ��У���Ϊֻ��ֱ����������ѡ���ܱ�������
    //�ø�������������������ѡ��Ĭ����Ȼ���Ա�������
    //��˿���ͨ�����Ϊ�ǻ���·���ϵ�ѡ�������ʵ������

    public UiPanel mCtrlDp;
    public UiList mStartListDp;

    UiList mListFocus;
    UiList mListLastFocus;
    Dictionary<UiList, EStateSubIn> mSubLists;

    bool mInPause;

    internal void FollowAcceptOneSubList(UiList one)
    {
        if (mSubLists == null)
            mSubLists = new Dictionary<UiList, EStateSubIn>();
        one.gameObject.SetActive(false);
        mSubLists.Add(one, EStateSubIn.Close);
    }

    internal void FollowUse()
    {
        if (mStartListDp != null)
        {
            mListFocus = mStartListDp;
            mListFocus.MakeCome();
        }
    }

    internal void FollowDown()
    {
        mListFocus.MakeDown();
        mListLastFocus = mListFocus;
    }

    internal void FollowPause()
    {
        if (!mInPause) ;
    }

    internal void FollowContinue()
    {
        
    }

    //=================================

    internal void TakeDown()
    { }

    internal void TakeLeave()
    { }

    internal void TakeCome(UiPanel to)
    {

    }//��ĳ����壬�����ǵ�ǰδ�������������

    internal void TakeCome(UiList to)
    {

    }

    //=================================

    internal bool SuWhetherHave(UiList to)
    {
        return mSubLists.ContainsKey(to);
    }

}
