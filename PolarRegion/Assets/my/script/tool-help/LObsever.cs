using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//观察者模式，多对多，特殊化，高性能
//参考系统自带实现的观察者模式代码，然后自己复制改写成的

public interface IObser<V>//泛型是被观察者的类型
{
    void OnInvalid(V obvable);//被观察者告知它自己失效了

    void OnNext(V obvable,InfoSeal ifo);//被观察者向观察者通知一些消息

    ReadyStopObserToAll neReadyStopObserToAll { get; set; }
}

public interface IObvableL<V>//泛型是被观察者自己的类型
{
    void NuAcceptObser(IObser<V> obser);
    //需要将obser参数加入到BroadcastToObser<V>中

    BroadcastToObser<V> neObsersHave { get; set; }
}

public interface IObserBreakL//供观察者，能主动移除某被观察者对自己需要的通知
{
    void NuDispose();
}

public class BroadcastToObser<V>
{
    List<IObser<V>> obsers;//只能由观察者自己通过接口选择从该列表中移除

    V obvableThis;

    public BroadcastToObser(V obvableThis)
    {
        obsers = new List<IObser<V>>();
        this.obvableThis = obvableThis;
    }

    public void NuAcceptNew(IObser<V> obser)
    {
        if (!obsers.Contains(obser))
        {
            obsers.Add(obser);
            obser.neReadyStopObserToAll.AcceptNew(new ObserBreakL<V>(obsers, obser));
        }
    }

    public void NuOnInvalid()
    {
        foreach (IObser<V> obser in obsers)
        {
            obser.OnInvalid(obvableThis);
        }
    }

    public void NuOnNext(InfoSeal ifo)
    {
        foreach (IObser<V> obser in obsers)
        {
            obser.OnNext(obvableThis, ifo);
        }
    }
}

public class ReadyStopObserToAll
{
    List<IObserBreakL> ReadyBreaks;

    public ReadyStopObserToAll()
    {
        ReadyBreaks = new List<IObserBreakL>();
    }

    public void AcceptNew(IObserBreakL obvable)
    {
        if (!ReadyBreaks.Contains(obvable))
            ReadyBreaks.Add(obvable);
    }

    public void NuMake()
    {
        foreach (IObserBreakL one in ReadyBreaks)
        {
            one.NuDispose();
        }
    }
}

public class ObserBreakL<V> : IObserBreakL
{
    List<IObser<V>> obsers;//被观察者对所有观察者的引用列表
    IObser<V> obserThis;//这一次观察申请的对方

    public ObserBreakL(List<IObser<V>> obsers, IObser<V> obserthis)
    {
        this.obsers = obsers;
        this.obserThis = obserthis;
    }

    public void NuDispose()//当观察者不再接收通知时，可调用Dispose函数取消订阅（反注册）
    {
        //obserThis != null
        if (obsers.Contains(obserThis))
        {
            obsers.Remove(obserThis);
        }
    }
}