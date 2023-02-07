using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISyncMatchContent//����ͬ���ж�Ӧ��ϵ�Ĳ�ͬ���������ݣ�����ʹ�öԷ����б仯�����ȸ�С��ʡ����
{
    object FollowAdd(params object[] input);
    //����洢�˸ýӿڵ�A�������˱仯����ôA���øú����������ӿ�������B�ı仯
    //����ֵ����˼����B���������仯�Ľ�����ظ�A��һ��᷵�ع�A����B��һЩ��������

    void FollowDel(params object[] output);

    void FollowChange(params object[] fresh);
}//����ӿ�ֻ��˵�����⣬ʵ����������ã���������

//----------------------------

public interface ISyncMatchCollect<O>
{
    O FollowAdd(params object[] smc);//������������������

    void FollowDel(params object[] smc);
}

public interface ISyncMatchCollect<O, A>
{
    //ע�ⷺ�ͻ���ʹ�ýӿ���ʧȥͨ���ԡ������ԣ�������ϵ��ʹ��

    O FollowAdd(A a);

    void FollowDel(A a);
}

public interface ISyncMatchClectDL<O, A>//���ɾ���������һ��ɾ��
{
    O FollowAdd(A a);

    void FollowDel();
}

public interface ISyncMatchClectAL<O, A>//������ӣ������һ����������
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
    //���Ը������������Ϣ��������ͬ���⴦��Ȼ�󷵻ش���������ʹ�÷���֪
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

