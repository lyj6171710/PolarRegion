using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class IfoEventGbj
{
    public string nameGbj;
    public List<IfoEventBranch> branches;
}

[System.Serializable]
public class IfoEventBranch
{
    public string nameSub;
    public List<IfoOrder> seqEvents;
}

[System.Serializable]
public class IfoOrder
{
    public EPROrderComp kind;
    public string content;//内容转换到json后的状态
    //public object content；本来应该这样，但是好像不支持转换object变量中的object变量
}

public class PREventSave:MonoBehaviour
{
    //分别存储不同事件物体，每个事件物体，单独一个文件

    public List<GameObject> mEventsSaveNeed;

    IfoShelfFile mCurAttachToData;

    IfoEventGbj mCurDataEventGbj;

    void OnDisable()
    {
        //Save();
    }

    public void Save()
    {
        if (mCurAttachToData == null)
            mCurAttachToData = new IfoShelfFile();

        mCurAttachToData.super = "event";
        mCurAttachToData.withFolder = false;

        for (int i = 0; i < mEventsSaveNeed.Count; i++)
        {
            GameObject need = mEventsSaveNeed[i];
            mCurAttachToData.name = "event-" + need.name;
            VirtualDisk.It.AddFileToVtDisk(mCurAttachToData);

            mCurDataEventGbj = new IfoEventGbj();

            ProSaveCurNeed(need.transform);
        }

    }

    void ProSaveCurNeed(Transform cur)
    {
        mCurDataEventGbj.nameGbj = cur.gameObject.name;
        mCurDataEventGbj.branches = new List<IfoEventBranch>();
        for (int i = 0; i < cur.childCount; i++)
        {
            IfoEventBranch dataBranch = ProSaveBranchEvent(cur.GetChild(i).gameObject);
            mCurDataEventGbj.branches.Add(dataBranch);
        }

        VirtualDisk.It.SaveFileInRealDisk(mCurAttachToData, mCurDataEventGbj);
    }

    IfoEventBranch ProSaveBranchEvent(GameObject curOneSub)
    {
        IfoEventBranch dataBranch = new IfoEventBranch();
        dataBranch.nameSub = curOneSub.name;
        dataBranch.seqEvents = new List<IfoOrder>();

        PROrder[] instructs = curOneSub.GetComponents<PROrder>();
        //所得到组件的顺序，需要与显示时先后顺序一致。经验证，该系统接口符合要求
        for (int i = 0; i < instructs.Length; i++)
        {
            IfoOrder dataSec = new IfoOrder();
            dataSec.kind = instructs[i].ID;//组件类型
            dataSec.content = JsonUse.SuToJson(instructs[i].GetSave());//组件数据
            dataBranch.seqEvents.Add(dataSec);
        }

        return dataBranch;
    }
}
