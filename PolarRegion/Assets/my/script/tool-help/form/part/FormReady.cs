using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FormReady
{
    //��������Ϊ��Ա��ʹ��
    //���಻��գ���ʹ���������໥ǣ��

    //�����Ǹ����γ����岢ά�ֶ����Ĳٿع�ϵ

    public struct Ifo
    {
    }

    public Ifo meIfo;//��Ϣ��¼

    //֧��
    public GameObject meHold;//�����Ը�������Ч������
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
