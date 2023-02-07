using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObserverAccept : MonoBehaviour//简要版观察者模式
{
    //私用变量=========================================

    List<MonoBehaviour> observers;

    private void Awake()
    {
        observers = new List<MonoBehaviour>();
    }

    public void apply_observe_(MonoBehaviour callback)//参数是外界组件
    {
        bool exist = false;
        foreach (MonoBehaviour wait in observers)
            if (wait == callback) exist = true;
        if (!exist) observers.Add(callback);
    }

    public void divert_observe_(MonoBehaviour callback)
    {
        observers.Remove(callback);
    }

    public void send_msg_(string method,object para=null)
    {
        for (int i = 0; i < observers.Count; i++)
        {
            if (observers[i] == null) {
                observers.RemoveAt(i);
                i--;
            }
            else observers[i].SendMessage(method, para, SendMessageOptions.DontRequireReceiver);
        }
    }
}
