using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//所能填充函数所符合的格式，注意CallBack是对函数类型的指示
public delegate void CallBack();
public delegate void CallBack<T>(T arg);
public delegate void CallBack<T, X>(T arg1, X arg2);
public delegate void CallBack<T, X, Y>(T arg1, X arg2, Y arg3);
public delegate void CallBack<T, X, Y, Z>(T arg1, X arg2, Y arg3, Z arg4);
public delegate void CallBack<T, X, Y, Z, W>(T arg1, X arg2, Y arg3, Z arg4, W arg5);

public enum EventKindL//应该是，针对同一委托类型，可使对应不同函数
{//名称这样取，是为了避免名称冲突
    Mleft=0,Mright=1
}//标记特定的事件，随游戏程序需要，在这里增删改即可

public class EventNotify{//全局广播

    // 订阅广播性消息系统
    // 该系统中，消息申请者也是消息接收者，其需求的消息在外界可以随处可自产生来作用于它
    // 潜在问题：广播时需要知道相应可触发事件的参数顺序

    /*使用举例：
     * class A{
     *   private void Awake()
     *   {
     *       GetComponent<Button>().onClick.AddListener(() => {
     *      //按下按钮，发送广播
     *      Event_core.Broadcast(EventType.ShowText,"你好","吗",3.0F,6,5);});
     *   }
     * }
     * class B{
     *  private void Awake()
     *  {
     *      gameObject.SetActive(false);
     *      //添加广播监听，并绑定事件方法为Show
     *      Event_core.AddListener<string,string,float,int,int>(EventType.ShowText, Show);
     *  }
     *
     *  private void OnDestroy()
     *  {
     *      //移除广播监听
     *      Event_core.RemoveListener<string, string, float, int, int>(EventType.ShowText, Show);
     *  }
     *
     *  public void Show(string str, string str1, float a, int b, int c)
     *  {
     *      gameObject.SetActive(true);
     *      GetComponent<Text>().text = str + str1 + a + b + c;
     *  }
     * }
     */

    private static Dictionary<EventKindL, Delegate> mEventTable = new Dictionary<EventKindL, Delegate>();//字典结构适合对应关系

    private static void OnListenerAdding(EventKindL eventType, Delegate callBack)//添加监听，当加入监听时的准备工作
    {
        if (!mEventTable.ContainsKey(eventType))
        {
            mEventTable.Add(eventType, null);
        }
        Delegate d = mEventTable[eventType];
        if (d != null && d.GetType() != callBack.GetType())
        {
            throw new Exception(string.Format("尝试为事件{0}添加不同类型的委托，当前事件所对应的委托是{1}，要添加的委托类型为{2}", eventType, d.GetType(), callBack.GetType()));
        }
    }
    private static void OnListenerRemoving(EventKindL eventType, Delegate callBack)//移除监听，当取消监听时的准备工作
    {
        if (mEventTable.ContainsKey(eventType))
        {
            Delegate d = mEventTable[eventType];
            if (d == null)
            {
                throw new Exception(string.Format("移除监听错误：事件{0}没有对应的委托", eventType));
            }
            else if (d.GetType() != callBack.GetType())
            {
                throw new Exception(string.Format("移除监听错误：尝试为事件{0}移除不同类型的委托，当前委托类型为{1}，要移除的委托类型为{2}", eventType, d.GetType(), callBack.GetType()));
            }
        }
        else
        {
            throw new Exception(string.Format("移除监听错误：没有事件码{0}", eventType));
        }
    }
    private static void OnListenerRemoved(EventKindL eventType)//当取消监听后的后续工作
    {
        if (mEventTable[eventType] == null)//刷新监听列表
        {
            mEventTable.Remove(eventType);
        }
    }

    //no parameters
    public static void AddListener(EventKindL eventType, CallBack callBack)
    {
        OnListenerAdding(eventType, callBack);
        mEventTable[eventType] = (CallBack)mEventTable[eventType] + callBack;
    }//事件码申请时需给予所关联的函数来相互绑定
    //Single parameters
    public static void AddListener<T>(EventKindL eventType, CallBack<T> callBack)
    {
        OnListenerAdding(eventType, callBack);
        mEventTable[eventType] = (CallBack<T>)mEventTable[eventType] + callBack;
    }
    //two parameters
    public static void AddListener<T, X>(EventKindL eventType, CallBack<T, X> callBack)
    {
        OnListenerAdding(eventType, callBack);
        mEventTable[eventType] = (CallBack<T, X>)mEventTable[eventType] + callBack;
    }
    //three parameters
    public static void AddListener<T, X, Y>(EventKindL eventType, CallBack<T, X, Y> callBack)
    {
        OnListenerAdding(eventType, callBack);
        mEventTable[eventType] = (CallBack<T, X, Y>)mEventTable[eventType] + callBack;
    }
    //four parameters
    public static void AddListener<T, X, Y, Z>(EventKindL eventType, CallBack<T, X, Y, Z> callBack)
    {
        OnListenerAdding(eventType, callBack);
        mEventTable[eventType] = (CallBack<T, X, Y, Z>)mEventTable[eventType] + callBack;
    }
    //five parameters
    public static void AddListener<T, X, Y, Z, W>(EventKindL eventType, CallBack<T, X, Y, Z, W> callBack)
    {
        OnListenerAdding(eventType, callBack);
        mEventTable[eventType] = (CallBack<T, X, Y, Z, W>)mEventTable[eventType] + callBack;
    }

    //no parameters
    public static void RemoveListener(EventKindL eventType, CallBack callBack)
    {
        OnListenerRemoving(eventType, callBack);
        mEventTable[eventType] = (CallBack)mEventTable[eventType] - callBack;
        OnListenerRemoved(eventType);
    }
    //single parameters
    public static void RemoveListener<T>(EventKindL eventType, CallBack<T> callBack)
    {
        OnListenerRemoving(eventType, callBack);
        mEventTable[eventType] = (CallBack<T>)mEventTable[eventType] - callBack;
        OnListenerRemoved(eventType);
    }
    //two parameters
    public static void RemoveListener<T, X>(EventKindL eventType, CallBack<T, X> callBack)
    {
        OnListenerRemoving(eventType, callBack);
        mEventTable[eventType] = (CallBack<T, X>)mEventTable[eventType] - callBack;
        OnListenerRemoved(eventType);
    }
    //three parameters
    public static void RemoveListener<T, X, Y>(EventKindL eventType, CallBack<T, X, Y> callBack)
    {
        OnListenerRemoving(eventType, callBack);
        mEventTable[eventType] = (CallBack<T, X, Y>)mEventTable[eventType] - callBack;
        OnListenerRemoved(eventType);
    }
    //four parameters
    public static void RemoveListener<T, X, Y, Z>(EventKindL eventType, CallBack<T, X, Y, Z> callBack)
    {
        OnListenerRemoving(eventType, callBack);
        mEventTable[eventType] = (CallBack<T, X, Y, Z>)mEventTable[eventType] - callBack;
        OnListenerRemoved(eventType);
    }
    //five parameters
    public static void RemoveListener<T, X, Y, Z, W>(EventKindL eventType, CallBack<T, X, Y, Z, W> callBack)
    {
        OnListenerRemoving(eventType, callBack);
        mEventTable[eventType] = (CallBack<T, X, Y, Z, W>)mEventTable[eventType] - callBack;
        OnListenerRemoved(eventType);
    }
    
    //no parameters
    public static void Broadcast(EventKindL eventType)
    {
        Delegate d;
        if (mEventTable.TryGetValue(eventType, out d))
        {
            CallBack callBack = d as CallBack;
            if (callBack != null)
            {
                callBack();
            }
            else
            {
                throw new Exception(string.Format("广播事件错误：事件{0}对应委托具有不同的类型", eventType));
            }
        }
    }//外界广播时，先出写标定事件(由事件码驱动)
    //single parameters
    public static void Broadcast<T>(EventKindL eventType, T arg)
    {
        Delegate d;
        if (mEventTable.TryGetValue(eventType, out d))
        {
            CallBack<T> callBack = d as CallBack<T>;
            if (callBack != null)
            {
                callBack(arg);
            }
            else
            {
                throw new Exception(string.Format("广播事件错误：事件{0}对应委托具有不同的类型", eventType));
            }
        }
    }//外界广播时，先出写标定事件，然后传递参数给相应函数
    //two parameters
    public static void Broadcast<T, X>(EventKindL eventType, T arg1, X arg2)
    {
        Delegate d;
        if (mEventTable.TryGetValue(eventType, out d))
        {
            CallBack<T, X> callBack = d as CallBack<T, X>;
            if (callBack != null)
            {
                callBack(arg1, arg2);
            }
            else
            {
                throw new Exception(string.Format("广播事件错误：事件{0}对应委托具有不同的类型", eventType));
            }
        }
    }
    //three parameters
    public static void Broadcast<T, X, Y>(EventKindL eventType, T arg1, X arg2, Y arg3)
    {
        Delegate d;
        if (mEventTable.TryGetValue(eventType, out d))
        {
            CallBack<T, X, Y> callBack = d as CallBack<T, X, Y>;
            if (callBack != null)
            {
                callBack(arg1, arg2, arg3);
            }
            else
            {
                throw new Exception(string.Format("广播事件错误：事件{0}对应委托具有不同的类型", eventType));
            }
        }
    }
    //four parameters
    public static void Broadcast<T, X, Y, Z>(EventKindL eventType, T arg1, X arg2, Y arg3, Z arg4)
    {
        Delegate d;
        if (mEventTable.TryGetValue(eventType, out d))
        {
            CallBack<T, X, Y, Z> callBack = d as CallBack<T, X, Y, Z>;
            if (callBack != null)
            {
                callBack(arg1, arg2, arg3, arg4);
            }
            else
            {
                throw new Exception(string.Format("广播事件错误：事件{0}对应委托具有不同的类型", eventType));
            }
        }
    }
    //five parameters
    public static void Broadcast<T, X, Y, Z, W>(EventKindL eventType, T arg1, X arg2, Y arg3, Z arg4, W arg5)
    {
        Delegate d;
        if (mEventTable.TryGetValue(eventType, out d))
        {
            CallBack<T, X, Y, Z, W> callBack = d as CallBack<T, X, Y, Z, W>;
            if (callBack != null)
            {
                callBack(arg1, arg2, arg3, arg4, arg5);
            }
            else
            {
                throw new Exception(string.Format("广播事件错误：事件{0}对应委托具有不同的类型", eventType));
            }
        }
    }

    
}
