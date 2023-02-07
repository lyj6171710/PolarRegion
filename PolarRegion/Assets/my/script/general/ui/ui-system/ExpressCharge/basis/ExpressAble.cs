using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Microsoft.CSharp;
public abstract class ExpressAble : MonoBehaviour
{
    //һ���ɶ������������Ϣ��UIԪ��

    //ʹ��Express�ĳ����ǣ���������Ϸ�߼�����UIЧ���Ĺ���������ǿ��UIЧ���ĸ���

    //�㼶��ϵ��express��־���������ݡ��������ã������UI�������ݱ�����������

    public string nameFor;
    public Action NuWhenEnable { get; set; }//�ɿ�������ʱ��ʹ�ø�����ĸ������ִ��˳��

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
