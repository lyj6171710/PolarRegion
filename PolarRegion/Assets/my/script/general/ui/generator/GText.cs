using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GText : UiGenerate
{//帮助生成一个文本ui，生成后靠申请生成者自己管理

    public static GText It;

    protected override void AwakeOther()
    {
        It = this;
    }

    //外界可用====================================

    public GameObject SuForm(string show, int style, UiAlone.Ifo refer, float size = 1)
    {
        AloneText text = FormAlone(style, refer, size).gameObject.AddComponent<AloneText>();
        text.MakeReady(show);
        return text.gameObject;
    }
    
    //内部工具======================================
    
}
