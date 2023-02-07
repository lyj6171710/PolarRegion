using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//ͬ��ͬһ����Ķ��order������һ���¼�

public enum EPROrderComp { dialogue,test }

public abstract class PROrder : MonoBehaviour
{
    public abstract EPROrderComp ID { get; }

    public abstract void Run();

    public abstract bool IfFinish();

    public abstract object GetSave();

    public abstract void LoadData(object dataSave);
}
