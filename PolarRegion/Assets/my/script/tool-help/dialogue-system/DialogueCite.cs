using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[System.Serializable]
public class DataTalkOne
{
    public string name;
    public string speech;
    [PreviewField] public Sprite head;//这里其实更应该是字符串变量，然后存资源路径
    //不过发现Unity支持这样存，然后还原引用关系，只要被引用资源地址没变
    public List<DataTalkOneSelect> selects;
}

[System.Serializable]
public class DataTalkOneSelect
{
    public string intent;//意图
    public GameObject nextBranch;//跳转到另一个分支
                                 //每一个分支，应由一个物体承担
                                 //另外这么做，是为利用起编辑器自带的跳转提示功能，不然就难以直观化显示数据的相互引用关系了

    [HideInInspector]
    public string citeTo;//分支所处物体的名称
                         //引用关系，是存储不了的，因为被引用物体是动态生成销毁的，所以这里需要存名称
                         //名称与对应物体，可以相互生成，直观操作时应只需赋值物体，不需管这个名称变量
}

[System.Serializable]
public class DataTalkBranch//管理按顺序发生的对话，直到出现分支（但也不一定就会拐向其它分支）
{
    public List<DataTalkOne> seq;

    public int meLengh { get { return seq.Count; } }
}

public interface IDataTalkBranchGet
{
    public DataTalkBranch meDataTalkBranch { get;}
}

public class DialogueCite
{
}