using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PREventExe : MonoBehaviour
{
    public GameObject TestToExe;

    private void Start()
    {
        MakeRun(TestToExe.transform.GetChild(0).gameObject);
    }

    //==========================

    bool mInRun;
    Coroutine mRuningUnit;

    bool mFinishSec;
    Coroutine mRunningUnit;

    public void MakeRun(GameObject eventBranch)
    {
        mRuningUnit = StartCoroutine(Run(eventBranch));
    }

    IEnumerator Run(GameObject eventBranch)
    {
        //Debug.Log("startExe");
        mInRun = true;
        PROrder[] instructs = eventBranch.GetComponents<PROrder>();
        //�õ��Ľ����Ҫ�ǰ�����ʾ���Ⱥ�˳��ͬ����˳�����кõ�
        for (int i = 0; i < instructs.Length; i++)
        {
            mRunningUnit = StartCoroutine(Run(instructs[i]));
            while (!mFinishSec) yield return null;
        }
        mInRun = false;
        //Debug.Log("finishExe");
    }

    IEnumerator Run(PROrder wait)
    {
        mFinishSec = false;
        wait.Run();
        while (!wait.IfFinish())yield return null;
        mFinishSec = true;
    }


    //===========================

    public static PREventExe It;


    private void Awake()
    {
        It = this;
    }

}
