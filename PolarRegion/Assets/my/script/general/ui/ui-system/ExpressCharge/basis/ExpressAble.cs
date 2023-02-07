using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Microsoft.CSharp;
public abstract class ExpressAble : MonoBehaviour
{
    //一个可独立表达所给信息的UI元件

    //使用Express的初衷是，不想让游戏逻辑考虑UI效果的构建，并且强化UI效果的复用

    //层级关系：express标志》数据依据》数据运用，外界向UI传达数据表达需求就是了

    public string nameFor;
    public Action NuWhenEnable { get; set; }//可控制启动时，使用该组件的各组件的执行顺序

    //========================================

    public ExprReferArray Array => this as ExprReferArray;

    public ExprReferInt Int => this as ExprReferInt;

    public ExprReferFloat Float => this as ExprReferFloat;

    public ExprReferImg Img => this as ExprReferImg;

    public ExprReferStr Str => this as ExprReferStr;


    //========================================

    bool mHaveReady;

    protected virtual void MakeReady() 
    {
        NuWhenEnable += () => { };
        mHaveReady = true;
    }

    void Awake()
    {
        MakeReady();
    }

    void Start()
    {
        NuWhenEnable();
    }

    void OnEnable()
    {
        NuWhenEnable();
    }
}
