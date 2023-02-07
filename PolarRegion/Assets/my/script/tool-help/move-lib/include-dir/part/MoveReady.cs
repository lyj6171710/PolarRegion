using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class MoveReady//�ƶ�׼�����ƶ�������Ĳ����ڲ����ܣ���ͨ�ģ�������ȡ����
{
    //��������Ϊ��Ա��ʹ��
    //���಻��գ���ʹ���������໥ǣ��

    //��Щ�ƶ������У����õ�Ч��������ֱ�����ƶ����ܳа�

    public struct Ifo//Խֻ�а��ƶ���
    {
        public Vector2 dir;
        //ͼ��ӳ����ң�ת���µķ���
        //��Գ������ƶ�����
        public float speed;
        public float span;
    }

    public struct IfoImg
    {
        public Transform mount;//ͼ��������һ��������
        public Sprite hold;
        public int angleSelf;//������ҷ�����
        //ԭ�еĽǶȣ�����������������Ȼ�󴫽���
        //ͼ������ı����������ҵĽǶ�
        public bool flip;
    }

    public IfoImg meIfoImg;//��Ϣ��¼
    public Ifo meIfo;

    //֧��
    public GameObject meMotion;//�����Ը�������Ч������
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

        //����
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
