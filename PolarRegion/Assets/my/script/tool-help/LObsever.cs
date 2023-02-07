using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//�۲���ģʽ����Զ࣬���⻯��������
//�ο�ϵͳ�Դ�ʵ�ֵĹ۲���ģʽ���룬Ȼ���Լ����Ƹ�д�ɵ�

public interface IObser<V>//�����Ǳ��۲��ߵ�����
{
    void OnInvalid(V obvable);//���۲��߸�֪���Լ�ʧЧ��

    void OnNext(V obvable,InfoSeal ifo);//���۲�����۲���֪ͨһЩ��Ϣ

    ReadyStopObserToAll neReadyStopObserToAll { get; set; }
}

public interface IObvableL<V>//�����Ǳ��۲����Լ�������
{
    void NuAcceptObser(IObser<V> obser);
    //��Ҫ��obser�������뵽BroadcastToObser<V>��

    BroadcastToObser<V> neObsersHave { get; set; }
}

public interface IObserBreakL//���۲��ߣ��������Ƴ�ĳ���۲��߶��Լ���Ҫ��֪ͨ
{
    void NuDispose();
}

public class BroadcastToObser<V>
{
    List<IObser<V>> obsers;//ֻ���ɹ۲����Լ�ͨ���ӿ�ѡ��Ӹ��б����Ƴ�

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
    List<IObser<V>> obsers;//���۲��߶����й۲��ߵ������б�
    IObser<V> obserThis;//��һ�ι۲�����ĶԷ�

    public ObserBreakL(List<IObser<V>> obsers, IObser<V> obserthis)
    {
        this.obsers = obsers;
        this.obserThis = obserthis;
    }

    public void NuDispose()//���۲��߲��ٽ���֪ͨʱ���ɵ���Dispose����ȡ�����ģ���ע�ᣩ
    {
        //obserThis != null
        if (obsers.Contains(obserThis))
        {
            obsers.Remove(obserThis);
        }
    }
}