using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ICopySelf<T>//���ڽ�����ù�ϵ
{
    public T GetCopy();//��Ȼ���ʹ��ʱ��Ӧ��֪����ʲô�������ͣ�������ʡ��������
}

public abstract class Srv<T> where T : class//���ж����ṩ����Ľӿ�
{//service
    protected T j;
    public Srv(T belong){ j = belong; }
}