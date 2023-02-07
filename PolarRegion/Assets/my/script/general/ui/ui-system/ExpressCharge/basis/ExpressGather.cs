using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ExpressGather : MonoBehaviour {//收集各项显示的承包者

    //面向场景中使用UI的各种物体

    //======================================

    public ExpressAble this[string name] { get { return dic[name]; } }

    public List<ExpressAble> mGatherDp;//各子面板

    //-------------------------------------
    public bool meInOpen { get { return mInOpen; } }

    bool mInOpen;

    public void SuInputAction(Action open, Action close, Action tick)
    {
        mOpenChart += open;
        mUpdateChart += tick;
        mCloseChart += close;
    }

    public void SuOpen()
    {
        if (!mInOpen)
        {
            gameObject.SetActive(true);
            mOpenChart();
            mInOpen = true;
        }
    }

    public void SuClose()
    {
        if (mInOpen)
        {
            gameObject.SetActive(false);
            mCloseChart();
            mInOpen = false;
        }
    }

    public void SuOpen(string name) {
        if (!this[name].gameObject.activeSelf)
            this[name].gameObject.SetActive(true);
    }

    public void SuClose(string name) {
        if (this[name].gameObject.activeSelf)
            this[name].gameObject.SetActive(false);
    }

    //==========================

    Action mOpenChart;
    Action mCloseChart;
    Action mUpdateChart;

    void Update()
    {
        mUpdateChart();
    }

    void OnEnable()
    {
        SuOpen();
    }

    void OnDisable()
    {
        SuClose();
    }

    void Awake()
    {
        mOpenChart += () => { };
        mUpdateChart += () => { };
        mCloseChart += () => { };
        Convert();
    }

    Dictionary<string, ExpressAble> dic;
    bool mHaveConvert;

    void Convert()
    {
        if (!mHaveConvert)
        {
            dic = new Dictionary<string, ExpressAble>();
            foreach (ExpressAble express in mGatherDp)
            {
                dic.Add(express.nameFor, express);
            }
            mHaveConvert = true;
        }
    }
}
