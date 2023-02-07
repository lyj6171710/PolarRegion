using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrefabCite : MonoBehaviour
{
    public GameObject SelfTemplate;
    //挂载该组件的物体，要导出它自己的预制体，然后把这个预制体又复制到该组件上来

    //发觉最后，这个引用会变成引用所挂载物体自己，暂时没找到解决办法，
    //不过就算这样，只要所挂载物体保留着初始状态，且无额外初始化行为，外界使用起来，效果是一样的
}
