using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AlonePic : MonoBehaviour {//负责图片处理的功能

    //一次性赋值============================

    Sprite stuff;
    Sprite[] anim;
    
    //私用数据==============================
    
     Image image;

    //内外机制================================

    public void MakeReady(Sprite show)
    {
        stuff = show;
        image = gameObject.GetComponent<Image>();
        image.sprite = stuff;
        image.enabled = false;
    }

    public void ToBeAnim(Sprite[] sprites_cite)//转变成图像播放
    {
        anim = sprites_cite;
        StartCoroutine(PlayAnim());
    }

    //外界可用=====================================

    public void SuOpen()
    {
        image.enabled = true;
    }

    public void SuClose()
    {
        image.enabled = false;
    }

    public void SuMakeRotateTo(EToward4 to)//需确保所给图像，内含意义是朝下的
    {
        RectTransform tmp_rect = GetComponent<RectTransform>();
        switch (to)
        {
            case EToward4.down: tmp_rect.Rotate(new Vector3(0, 0, 0)); break;
            case EToward4.up: tmp_rect.Rotate(new Vector3(0, 0, 180)); break;
            case EToward4.left: tmp_rect.Rotate(new Vector3(0, 0, 270)); break;
            case EToward4.right: tmp_rect.Rotate(new Vector3(0, 0, 90)); break;
        }
    }
    
    //内部工具===============================

    IEnumerator PlayAnim()
    {
        while (!image.enabled) yield return null;
        int index = 0;
        while (true)
        {
            image.sprite = anim[index];
            if (index < anim.Length - 1) 
                index++;
            else
                index = 0;
            yield return new WaitForSeconds(0.3f);
        }
    }

}
