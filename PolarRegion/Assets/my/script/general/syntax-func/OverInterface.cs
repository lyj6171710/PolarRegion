using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ICopySelf<T>//用于解除引用关系
{
    public T GetCopy();//虽然外界使用时，应该知道是什么具体类型，但还是省性能优先
}

public abstract class Srv<T> where T : class//集中对外提供服务的接口
{//service
    protected T j;
    public Srv(T belong){ j = belong; }
}