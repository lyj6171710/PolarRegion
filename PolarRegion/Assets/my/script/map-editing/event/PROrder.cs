using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//同属同一物体的多个order，构成一个事件

public enum EPROrderComp { dialogue,test }

public abstract class PROrder : MonoBehaviour
{
    public abstract EPROrderComp ID { get; }

    public abstract void Run();

    public abstract bool IfFinish();

    public abstract object GetSave();

    public abstract void LoadData(object dataSave);
}
