using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GPic : UiGenerate {
    //相对标准视口，在指定位置，生成指定图片的部门

    public static GPic It;

    protected override void AwakeOther()
    {
        It = this;
    }

    //外界可用======================================

    public GameObject SuForm(Sprite show, int style, UiAlone.Ifo refer, float size = 1)
    {
        AlonePic pic = FormAlone(style, refer, size).gameObject.AddComponent<AlonePic>();
        pic.MakeReady(show);
        return pic.gameObject;
    }

    //内部工具===============================
    
}

[System.Serializable]
public struct AnimEffect
{
    public string name;
    public Sprite[] sequence;
    public float scale;
}