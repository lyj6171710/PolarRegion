using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FormReady
{
    //将该类作为成员来使用
    //该类不封闭，与使用它的类相互牵连

    //意义是负责形成物体并维持对它的操控关系

    public struct Ifo
    {
    }

    public Ifo meIfo;//信息记录

    //支持
    public GameObject meHold;//外界可以附加其它效果进来
    public SpriteRenderer meRender;
    public BoxCollider2D meCollid;

    public void ReadyShape(GameObject hold, Transform holder)
    {
        meHold = hold;

        GObject.BuildAndHold(meHold, holder,out meRender, out meCollid);

        SpriteAssist.SetCollidBySprite(meCollid, meRender);
    }

    public void SuSetOpacity(float ratio)
    {
        Vector4 former = meRender.color;
        meRender.color = new Color(former.x, former.y, former.z, ratio);
    }
}
