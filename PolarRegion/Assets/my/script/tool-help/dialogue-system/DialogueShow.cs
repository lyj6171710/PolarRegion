using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueShow : MonoBehaviour,ISwitchScene, AssistSelect.IUse
{
    //对话系统
    //dialogue系统框架层面
    //talk一次交谈
    //speech一次发言

    public bool meInUse => mInUse;

    bool mInUse;
    bool mInUseJust;
    DataTalkBranch mSeqUseNow;
    int mTurnIn;
    bool mInSelect;
    bool mHaveSelect;
    int mIndexSelect;

    ExpressGather mGather;
    AssistSelect mSelect;
    string optionsMain = "options";

    public void SuGo(DataTalkBranch saySeq)//可以打断
    {
        mGather = UiMager.It.meSingle.SuOpen(EChartSingle.dialogue);
        if (mGather == null) mGather = UiMager.It.meSingle.SuGetUse(EChartSingle.dialogue);

        if (mSelect == null)
        {
            mSelect = gameObject.AddComponent<AssistSelect>();
            mSelect.MakeReady(this, EToward4.up);
        }

        mTurnIn = 0;
        mSeqUseNow = saySeq;
        ShowByOrder(mTurnIn);
        mInUse = true;
        mInUseJust = true;
    }

    void Update()
    {
        if (mInUse)
        {
            if (mInUseJust)
                mInUseJust = false;//触发对话的那一帧，不能立即又跳转到下一条发言
            else
            {
                if (mInSelect)
                {
                    if (mHaveSelect)
                    {
                        mInSelect = false;
                        List<DataTalkOneSelect> selects = mSeqUseNow.seq[mTurnIn].selects;
                        if (selects[mIndexSelect].nextBranch != null)
                        {
                            SuGo(selects[mIndexSelect].nextBranch.GetComponent<IDataTalkBranchGet>().meDataTalkBranch);
                        }
                        else
                        {
                            ShowNextSpeech();
                        }
                    }
                }
                else
                {
                    if (UnifiedInput.It.meTapConfirm())
                    {
                        List<DataTalkOneSelect> selects = mSeqUseNow.seq[mTurnIn].selects;
                        if (selects != null && selects.Count != 0)
                        {
                            ShowAndCanSelect(selects);
                        }
                        else
                        {
                            ShowNextSpeech();
                        }
                    }
                }
            }
        }
    }

    void ShowNextSpeech()
    {
        if (mInUse)
        {
            mTurnIn++;
            if (mTurnIn < mSeqUseNow.meLengh)
                ShowByOrder(mTurnIn);
            else
            {
                UiMager.It.meSingle.SuClose(EChartSingle.dialogue);
                mInUse = false;
            }
        }
    }

    void ShowByOrder(int index)
    {
        ShowSpeech(mSeqUseNow.seq[index]);
        mGather.SuClose(optionsMain);
    }

    void ShowAndCanSelect(List<DataTalkOneSelect> options)
    {
        GameObject panel = ShowOptions(options);
        List<GameObject> blocks = panel.GetComponent<ExprNumItem.INowAble>().GetUsing();
        WaitDeal.It.Begin(() =>{
            mSelect.MakeUse(0, blocks, null);
        }, 0.25f);
        mInSelect = true;
        mHaveSelect = false;
    }

    public void iInformConfirmResult(int index)
    {
        mHaveSelect = true;
        mIndexSelect = index;
        mSelect.MakePause();
    }

    public void iInformBackNeed()
    {

    }

    //----------------------------

    void ShowSpeech(DataTalkOne talk)
    {
        mGather["name"].Str.SuChangeTo(talk.name);
        mGather["head"].Img.SuChangeTo(talk.head);
        mGather["content"].Str.SuChangeTo(talk.speech);
    }

    GameObject ShowOptions(List<DataTalkOneSelect> options)
    {
        mGather["options-num"].Int.SuChangeTo(options.Count);//选项数量
        for (int i = 0; i < options.Count; i++)
        {
            mGather["options-str"].Array.SuSetElemOne(i, options[i].intent);//选项内容
        }
        mGather.SuOpen(optionsMain);
        return mGather[optionsMain].gameObject;
    }

    //===========================

    public static DialogueShow It;

    public void WhenAwake()
    {
        It = this;
    }

    public void WhenSwitchScene()
    {
        
    }
}
