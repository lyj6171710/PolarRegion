using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PROrderTalkBranch : PROrder,IDataTalkBranchGet
{
    public DataTalkBranch mData;

    public DataTalkBranch meDataTalkBranch => mData;

    //=========================

    public override EPROrderComp ID => EPROrderComp.dialogue;

    public override void Run()
    {
        DialogueShow.It.SuGo(mData);
    }

    public override bool IfFinish()
    {
        return !DialogueShow.It.meInUse;
    }

    public override object GetSave()
    {
        foreach (DataTalkOne speech in mData.seq)
        {
            foreach (DataTalkOneSelect option in speech.selects)
            {
                if (option.nextBranch != null)
                    option.citeTo = option.nextBranch.name;
                else
                    option.citeTo = null;
            }
        }
        return mData;
    }

    public override void LoadData(object dataSave)
    {
        mData = dataSave as DataTalkBranch;
        if (mData != null)
        {
            foreach (DataTalkOne speech in mData.seq)
            {
                foreach (DataTalkOneSelect option in speech.selects)
                {
                    option.nextBranch = GbjAssist.FindSubGbjByName(transform.parent, option.citeTo);
                }
            }
        }
        else
            mData = new DataTalkBranch();
    }

    //==========================

    IfoShelfFile mDataAttach;

    void Awake()
    {
        
    }

    void OnDisable()
    {
        
    }

}
