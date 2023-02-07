using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Sirenix.OdinInspector;

public class UiList : MonoBehaviour, AssistSelect.IUse
{
    //ѡ���ṩ�ģ���ѡ�����á��رա�������ѡ���֧�ֽ�����Ϊ�Ĺ���
    //�����������Ϸ����Ӧ��ȡ���������ô��Ӧ����ѡ�����޹�

    //ѡ��Ƕ�ף�ѡ���ܰ���ѡ�����壬
    //��Ϊһ������£������ж���ѡ������ֱ�Ӳ��У����ڻ�ȡ
    //����Ҳ���Ǿ�û��Ƕ�׹��ܣ������Ƕ�ף������԰���ѡ��

    //==========================

    [HideInInspector]public UiPanel mSuper;//��ѡ���������

    public bool meInOpen => gameObject.activeSelf;
    public bool meInDown => gameObject.activeSelf && !mInUse;
    public bool meInUse => mInUse;
    
    bool mInUse;
    bool mInPause;
    bool mHaveReady;

    public void MakeReady()
    {
        ReadyBox();
        mHaveReady = true;
    }

    public void MakeUse()
    {
        StartSelect();
        mInUse = true;
    }

    public void MakePause()
    {
        mInPause = true;
        mSelect.MakePause();
    }

    public void MakeContinue()
    {
        mSelect.MakeContinue();
        mInPause = false;
    }

    public void MakeDown()
    {
        mSelect.MakeDown();
        mInUse = false;
    }//���Ա����ǣ���ر�

    //----------------------

    public void MakeCome()
    {
        if (!mHaveReady) MakeReady();
        if (!meInOpen) gameObject.SetActive(true);
        if (!mInUse) MakeUse();
        
    }//��ĳ������ѡ����Ҫ

    public void MakeLeave()
    {
        if (mInUse) MakeDown();
        if (meInOpen) gameObject.SetActive(false);
    }

    //================================

    public void TakeDown()
    {
        MakeDown();
    }

    public void TakeLeave()
    {
        MakeLeave();
    }

    public void TakeComeIncUp(UiPanel to)
    {
        if (mSuper.SuWhetherHave(to))
            mSuper.TakeFoward(to);
        else
            mSuper.TakeSideComeInUp(to);
    }//��ĳ����壬�����ǵ�ǰδ�������������

    public void TakeComeIncUp(UiList to)
    {
        if (mSuper.mListMager.SuWhetherHave(to))
            mSuper.mListMager.TakeCome(to);
        else
        {
            TakeComeIncUp(to.mSuper);
            if (to.mSuper.meInOpen)
                to.mSuper.mListMager.TakeCome(to);
        }
    }

    //====================================

    public bool mRememberSelectDp;
    [Required]public UiOption mStartOptionDp;
    [Required]public List<UiOption> mOptionsDp;

    AssistSelect mSelect;

    List<GameObject> mOptions;

    void ReadyBox()
    {
        mSelect = gameObject.AddComponent<AssistSelect>();
        mSelect.MakeReady(this, EToward4.up);
        mSelect.SuNotifyGoAboveList += () => { };//��ռ��λ��������
        mSelect.SuNotifyGoBelowList += () => { };

        mOptions = new List<GameObject>();
        for (int i = 0; i < mOptionsDp.Count; i++)
            mOptions.Add(mOptionsDp[i].gameObject);
    }

    void StartSelect()//����ѡ����
    {
        if (mRememberSelectDp && mOptionsDp[mSelect.meSelectCur])//����Ƿ��������л���
            mSelect.MakeUse(mOptionsDp[mSelect.meSelectCur].mIndex, mOptions, null);
        else if (mStartOptionDp) //����Ƿ�����½
            mSelect.MakeUse(mStartOptionDp.mIndex, mOptions, null);
    }

    //=======================================

    public bool mCanBackDp;
    public UnityEvent mCallWhenBackDp;//�Ӹ�ѡ�����ʱ�ἤ�����¼�

    public bool mLeaveWhenBackDp;
    public UiList mOnlyDvtCtrlToWhenBackDp;
    public UiPanel mMakePanelLeaveWhenBackDp;

    public bool mBackFmPanelDp;//�������������Ӧ���ز���

    private void Update()
    {
        if (mCanBackDp && UnifiedInput.It.meWhenBack())
            TurnBack();
    }

    void TurnBack()
    {
        if (mCanBackDp)//�ȿ��ܷ���ת
        {
            if (mCallWhenBackDp != null)//������������Ҫ����
                mCallWhenBackDp.Invoke();
            if (mSuper != null)//�ٿ�������תĿ�꣬û�оͲ���ת
            {
                //mLastSelect = mOptionsDp[mSelect.meSelectCur];
                //mSuperDp.MakeDivert(this, mDivertToListDp);
            }
        }

        //if (mLeaveWhenBackDp)
        //    MakeLeave();
        //else if (mDownWhenBackDp)
        //    MakeDown();
        //if (mDivertToWhenBackDp != null && mSuper != null)
        //    mSuper.TakeDivert(this, mDivertToWhenBackDp);
    }

    //=========================================

    [ContextMenu("�Զ��Ѽ���ѡѡ��")]
    void CollectOptionsHave()
    {//˼�룺�Ѽ�ѡ��Ĺ�����������¼����������Ĺ���
     //�ɴˣ��û�����ͨ������ѡ�����������������������Ʊ���¼����λ��
        mOptionsDp.Clear();
        for (int i = 0; i < transform.childCount; i++)
        {
            var optionOne = transform.GetChild(i).GetComponent<UiOption>();
            if (optionOne!=null)
            {//������������ض���ѡ�����
                optionOne.mSuper = this;//˳������ѡ��
                optionOne.mIndex = i;
                mOptionsDp.Add(optionOne);
            }
        }
    }

    public void iInformConfirmResult(int index)
    {
        mOptionsDp[index].MakeSure();
    }

    public void iInformBackNeed()
    {
        //ѡ���Լ�������ʶ�𷵻�
    }
}
