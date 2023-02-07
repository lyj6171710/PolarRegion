using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class WaitDeal : MonoBehaviour,ISwitchScene
{

    //-------------------------------------

    Dictionary<Coroutine, int> past;//关于为何不根据GameObject的开闭状态控制当前是否还执行？是因为可以立即关闭又立即打开
    Dictionary<Coroutine, Coroutine> unpack;

    public void BeginSafe(Action whenEnd, float delaySeconds, Int caseIn)
    {//针对第三个参数，用来帮助判断在延迟过后的那个当下，是否执行相关内容
        Coroutine alone = StartCoroutine(Alone(delaySeconds));//形成临时的唯一标识
        Coroutine wait = StartCoroutine(DelayCall(whenEnd, delaySeconds, alone, caseIn));//出发
        past.Add(wait, caseIn.at);//原来状态
        unpack.Add(alone, wait);//如果是用caseIn来标识相应等待过程，则只支持同时只有一个过程
    }

    public void BeginSafe(Action whenEnd, int frameNums, Int caseIn)
    {
        Coroutine alone = StartCoroutine(Alone(frameNums));
        Coroutine wait = StartCoroutine(DelayCall(whenEnd, frameNums, alone, caseIn));
        past.Add(wait, caseIn.at);
        unpack.Add(alone, wait);
    }

    IEnumerator Alone(float delaySeconds)
    {
        yield return new WaitForSeconds(delaySeconds + 0.1f);
    }

    IEnumerator Alone(int frameNums)
    {
        for (int i = 0; i < frameNums + 1; i++) 
            yield return null;
    }

    IEnumerator DelayCall(Action act, float delaySeconds,Coroutine selfClue, Int cur)
    {
        yield return new WaitForSeconds(delaySeconds);
        Act(act, selfClue, cur);
    }

    IEnumerator DelayCall(Action act, int frameNums, Coroutine selfClue, Int cur)
    {
        for (int i = 0; i < frameNums; i++)
            yield return null;
        Act(act, selfClue, cur);
    }

    void Act(Action act, Coroutine selfClue, Int cur)
    {
        Coroutine self = unpack[selfClue];
        if (cur.at == past[self]) act();
        past.Remove(self);
        unpack.Remove(selfClue);
    }

    //---------------------------------------

    public void Begin(Action whenEnd,int frameNums)//不安全，因为会执行，不管发起方是否已经关闭
    {
        StartCoroutine(DelayCall(whenEnd, frameNums));
    }

    public void Begin(Action whenEnd, float delaySeconds)
    {
        StartCoroutine(DelayCall(whenEnd, delaySeconds));
    }

    //---------------------------------------

    public static IEnumerator DelayCall(Action act, float delaySeconds)
    {//第一个参数是委托，第二个参数是延时等待的秒速
        yield return new WaitForSeconds(delaySeconds);
        act();
    }//外界自己操作这个协程

    public static IEnumerator DelayCall(Action act, int frameNums)
    {//第二个参数是等待的帧数
        for (int i = 0; i < frameNums; i++)
            yield return null;
        act();
    }
    //外界需要注意，整数式的浮点参数不加后缀f，会被认为是整型参数而调用该函数去了

    //架构需要===============================

    public static WaitDeal It;

    public void WhenAwake()
    {
        It = this;

        past = new Dictionary<Coroutine, int>();
        unpack = new Dictionary<Coroutine, Coroutine>();
    }

    public void WhenSwitchScene()
    {

    }

}