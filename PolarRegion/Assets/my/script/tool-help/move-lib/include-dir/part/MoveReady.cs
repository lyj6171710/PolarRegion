using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class MoveReady//移动准备，移动类组件的部分内部功能，共通的，所以提取出来
{
    //将该类作为成员来使用
    //该类不封闭，与使用它的类相互牵连

    //那些移动过程中，常用的效果，可以直接由移动功能承包

    public struct Ifo//越只承包移动的
    {
        public Vector2 dir;
        //图像从朝正右，转向新的方向
        //相对场景的移动方向
        public float speed;
        public float span;
    }

    public struct IfoImg
    {
        public Transform mount;//图像表达于哪一个物体上
        public Sprite hold;
        public int angleSelf;//相对正右方描述
        //原有的角度，外界相对正右描述，然后传进来
        //图像意义的本身朝向，离正右的角度
        public bool flip;
    }

    public IfoImg meIfoImg;//信息记录
    public Ifo meIfo;

    //支持
    public GameObject meMotion;//外界可以附加其它效果进来
    public SpriteRenderer meRender;
    public BoxCollider2D meCollid;

    public void MakeReady(GameObject move, IfoImg ifoImg)
    {
        meMotion = move;
        meIfoImg = ifoImg;

        meRender = meIfoImg.mount.GetComponent<SpriteRenderer>();
        meRender.enabled = false;

        meCollid = meIfoImg.mount.GetComponent<BoxCollider2D>();
        meCollid.isTrigger = true;
    }

    public void Go(Ifo ifo)
    {
        meIfo = ifo;

        //表现
        meRender.sprite = meIfoImg.hold;
        meRender.flipY = meIfoImg.flip;
        meRender.enabled = true;
        SuSetOpacity(1);

        SpriteAssist.SetCollidBySprite(meCollid, meRender);
    }

    public void SuSetOpacity(float ratio)
    {
        Vector4 former = meRender.color;
        meRender.color = new Color(former.x, former.y, former.z, ratio);
    }
}
