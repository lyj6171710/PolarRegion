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
    public string content;//����ת����json���״̬
    //public object content������Ӧ�����������Ǻ���֧��ת��object�����е�object����
}

public class PREventSave:MonoBehaviour
{
    //�ֱ�洢��ͬ�¼����壬ÿ���¼����壬����һ���ļ�

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
        //���õ������˳����Ҫ����ʾʱ�Ⱥ�˳��һ�¡�����֤����ϵͳ�ӿڷ���Ҫ��
        for (int i = 0; i < instructs.Length; i++)
        {
            IfoOrder dataSec = new IfoOrder();
            dataSec.kind = instructs[i].ID;//�������
            dataSec.content = JsonUse.SuToJson(instructs[i].GetSave());//�������
            dataBranch.seqEvents.Add(dataSec);
        }

        return dataBranch;
    }
}
