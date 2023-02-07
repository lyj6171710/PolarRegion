using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PREventRestore : MonoBehaviour
{
    IfoShelfFile mDataAttach;

    IfoEventGbj mDataEventGbj;

    public static PREventRestore It;

    void Awake()
    {
        It = this;

        if (mDataAttach == null)
            mDataAttach = new IfoShelfFile();

        mDataAttach.super = "event";
        mDataAttach.withFolder = false;
    }

    private void Start()
    {
        //GameObject test = LoadCertainEvent("user");
        //PREventExe.It.MakeRun(test.transform.GetChild(0).gameObject);
    }

    public GameObject LoadCertainEvent(string nameGbj)
    {
        mDataAttach.name = "event-" + nameGbj;
        VirtualDisk.It.AddFileToVtDisk(mDataAttach);
        VirtualDisk.It.LoadFileInRealDisk(mDataAttach, out mDataEventGbj);

        if (mDataEventGbj == null)
        {
            mDataEventGbj = new IfoEventGbj();
            mDataEventGbj.branches = new List<IfoEventBranch>();
        }

        return RestoreEventGbj(transform);
    }

    //========================================

    List<WaitLoad> mWaitLoad;

    GameObject RestoreEventGbj(Transform parent)
    {
        mWaitLoad = new List<WaitLoad>();
        GameObject eventGbj = GbjAssist.AddChildNormal(parent, mDataEventGbj.nameGbj);

        for (int i = 0; i < mDataEventGbj.branches.Count; i++)
        {
            IfoEventBranch dataBranch = mDataEventGbj.branches[i];
            RestoreEventBranch(eventGbj.transform, dataBranch);
        }

        for (int i = 0; i < mWaitLoad.Count; i++)
        {//��֧��������ٽ���������֧����Ϊ��֧���ݿ��ܰ����������֧�����ã�����Ҫ���з�֧�ṹ����
            mWaitLoad[i].unit.LoadData(mWaitLoad[i].content);
        }

        return eventGbj;
    }

    void RestoreEventBranch(Transform belong, IfoEventBranch dataBranch)
    {
        GameObject eventBranch = GbjAssist.AddChildNormal(belong, dataBranch.nameSub);
        for (int i = 0; i < dataBranch.seqEvents.Count; i++)
        {
            IfoOrder dataUnit = dataBranch.seqEvents[i];
            RestoreEventUnit(eventBranch, dataUnit);
        }
    }

    void RestoreEventUnit(GameObject belong, IfoOrder dataUnit)
    {
        PROrder unit = null;
        object content = null;
        switch (dataUnit.kind)
        {
            case EPROrderComp.dialogue:
                unit = belong.AddComponent<PROrderTalkBranch>();
                DataTalkBranch dataTalkBranch;//���������ʽ����������Ϣ����Ȼ������ȷ��ȡ����
                if (JsonUse.SuFromJson(dataUnit.content, out dataTalkBranch))
                    content = dataTalkBranch;
                break;
            case EPROrderComp.test:
                break;
            default:
                break;
        }
        if (unit != null && content != null)
            mWaitLoad.Add(new WaitLoad(unit, content));
    }

    struct WaitLoad
    {
        public PROrder unit;
        public object content;

        public WaitLoad(PROrder un, object co)
        {
            unit = un;
            content = co;
        }
    }
}
