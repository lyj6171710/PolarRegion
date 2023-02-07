using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISyncMatchContent//用于同步有对应关系的不同事物间的内容，可以使得对方需有变化的粒度更小，省性能
{
    object FollowAdd(params object[] input);
    //假设存储了该接口的A，发生了变化，那么A调用该函数，引发接口所属者B的变化
    //返回值的意思就是B将它发生变化的结果返回给A，一般会返回供A操作B的一些线索对象

    void FollowDel(params object[] output);

    void FollowChange(params object[] fresh);
}//这个接口只是说明问题，实际情况不好用，不建议用

//----------------------------

public interface ISyncMatchCollect<O>
{
    O FollowAdd(params object[] smc);//参数名用来利于搜索

    void FollowDel(params object[] smc);
}

public interface ISyncMatchCollect<O, A>
{
    //注意泛型会让使用接口者失去通用性、独立性，特殊联系才使用

    O FollowAdd(A a);

    void FollowDel(A a);
}

public interface ISyncMatchClectDL<O, A>//如果删除，从最后一个删除
{
    O FollowAdd(A a);

    void FollowDel();
}

public interface ISyncMatchClectAL<O, A>//如果增加，在最后一个后面增加
{
    O FollowAdd();

    void FollowDel(A a);
}

public interface ISyncMatchCollect<O, A, B>
{
    O FollowAdd(A a);

    void FollowDel(B b);
}

public interface ISyncMatchClectMsgAL<O, A, B>
{
    O FollowAdd();

    void FollowDel(A a);

    B MsgFmUser(params object[] args);
    //可以根据情况发送消息，来做不同特殊处理，然后返回处理结果，让使用方得知
}

//--------------------------------

public interface ISyncMatchAdd<O>
{
    O FollowAdd(params object[] sma);
}

public interface ISyncMatchAdd<O,A>
{
    O FollowAdd(A a);
}

public interface ISyncMatchAdd<O,A,B>
{
    O FollowAdd(A a,B b);
}

public interface ISyncMatchAdd<O,A,B,C>
{
    O FollowAdd(A a,B b,C c);
}

//------------------------------------
public interface ISyncMatchDel
{
    void FollowDel(params object[] smd);
}

public interface ISyncMatchDel<A>
{
    void FollowDel(A a);
}

public interface ISyncMatchDel<A,B>
{
    void FollowDel(A a,B b);
}
public interface ISyncMatchDel<A,B,C>
{
    void FollowDel(A a,B b,C c);
}

//------------------------------------

public interface ISyncMatchChange
{
    void FollowChange(params object[] smch);
}

public interface ISyncMatchChange<A>
{
    void FollowChange(A a);
}

public interface ISyncMatchChange<A,B>
{
    void FollowChange(A a,B b);
}

