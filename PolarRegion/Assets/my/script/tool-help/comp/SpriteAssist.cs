using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SpriteAssist
{
    public static Vector2 GetSizeInScene(Sprite sprite)
    {
        float x = sprite.rect.width / sprite.pixelsPerUnit;
        float y = sprite.rect.height / sprite.pixelsPerUnit;
        return new Vector2(x, y);
    }

    public static void SetCollidBySprite(BoxCollider2D collid, SpriteRenderer render)//Ĭ�����������λ��ͬһ������
    {
        Vector2 sizeBody = GetSizeInScene(render.sprite);
        collid.size = sizeBody;
    }

    //==========================================

    public static void MakeSibAlignToButtom(Transform sib,Transform self)
    {
        //����selfͼ�����ľ���self������λ��

        float scaleSelfY = self.localScale.y;
        float scaleSibY = sib.localScale.y;

        Vector2 sizeSib = GetSizeInScene(sib.GetComponent<SpriteRenderer>().sprite);
        Vector2 sizeSelf = GetSizeInScene(self.GetComponent<SpriteRenderer>().sprite);

        Vector3 localPos = sib.localPosition;
        sib.localPosition = new Vector3(localPos.x, 
            sizeSib.y / 2 * scaleSibY - sizeSelf.y / 2 * scaleSelfY, localPos.z);
        //�����Ѿ��׶˶��룬Ȼ�����sib���self��ƫ����
    }

    //=========================================

    //һ��ģ�壬������ͱ�ʾ��λ�ã������������壬һ����ʾ��Ӱ��һ����ʾ����
    //�������Ӱ�ȿ������壬��Ӱ�߶���������ķ�֮һ����Ӱλ����ԭ��һ��
    //�����С��ԭ��һ�£�����ᱻ��Ӱ�����ƶ���ֱ������ײ�����Ӱ�ײ���ƽ
    //��Ӱ��һ����ײ�壬Ҫ����ײ��������ȿ���Ⱥ�����һ�����߶�������һ�룬�ײ�������ײ���ƽ

    public struct Ifo
    {
        public float scaleShrinkX;
        public float scaleShrinkY;
    }

    public static void VaryScaleFitToSibLook(Transform pawn, Transform shadow, Ifo ifo)
    {
        //pawn��ʾ���壬����һ��Ҫ�ͱ�ʾ��Ӱ������ͬ����ͬ��

        Vector2 sizeBody = GetSizeInScene(pawn.GetComponent<SpriteRenderer>().sprite);
        Vector2 sizeShadow = GetSizeInScene(shadow.GetComponent<SpriteRenderer>().sprite);
        //֪����żռ�����Ŀ�֪���Լ�ռ�����Ŀ�Ȼ��ɵõ�����
        float ratioWidth = sizeBody.x / ifo.scaleShrinkX / sizeShadow.x;//��Ӱ��ȵ�����ż���n��֮һ
        float ratioHeight = sizeBody.y / ifo.scaleShrinkY / sizeShadow.y;//��Ӱ�߶ȵ�����ż�߶�n��֮һ
        shadow.localScale = new Vector3(ratioWidth, ratioHeight);//��Ӱ��ʵ���ڸ������ϣ�ֻ����ŵ�λ��ͬ������

        MakeSibAlignToButtom(pawn, shadow);
        //����ż����ƫ�ƣ�ֱ���ײ�����Ӱ�ײ���ƽ
    }

    public static void VaryCollidToFitSibLook(Transform pawn, Transform shadow, Ifo ifo)
    {
        //foot��ʾ�����壬��һ��Ҫ�ǵ�һ�������壬���Բ���ͬshadow.parent

        Vector2 sizeBody = GetSizeInScene(pawn.GetComponent<SpriteRenderer>().sprite);
        sizeBody *= GbjAssist.GetSumScaleWhenSelf(pawn);//ʵ�����
        float ratioWidth = shadow.localScale.x;
        float ratioHeight = shadow.localScale.y;

        BoxCollider2D collidShadow = shadow.GetComponent<BoxCollider2D>();
        //��Ӱ����ײ��������������н���ͨ·��������ײ���СӦ������ͬ��
        collidShadow.size = new Vector2(
            sizeBody.x / ifo.scaleShrinkX / ratioWidth, 
            sizeBody.y / ifo.scaleShrinkY / ratioHeight);//��Ӱ��ײ��߶ȵ�����ż�߶ȵ�n��֮һ
        //��scaleΪ1ʱ����ײ�ؿ�ĳ��ȵ�λ�ĳ���Ҳ����1����scale��Ϊ1ʱ���ؿ�ĵ�λ���Ȼᷢ���仯
        float bodyFootAt = pawn.position.y  - sizeBody.y / 2;//��ż���λ���Ѿ������仯
        //����ż�ײ���λ�����ڣ�����ǰʵ��Ӧ����������
        float collidFootAt = shadow.position.y - collidShadow.size.y / 2 * ratioHeight;
        collidShadow.offset = new Vector2(0, (bodyFootAt - collidFootAt) / ratioHeight);

    }

    //----------------------------------------------

    //һ��ģ�壬����������Ӱ�������������壬�������Ӱ�ȿ������壬��Ӱ�߶���������ķ�֮һ��
    //��Ӱ�ײ�������ײ���ƽ�������С��ԭ��һ�£���Ӱλ����ԭ��һ��
    //��Ӱͬʱ����һ����ײ�壬Ҫ����ײ���ȵ������壬�߶ȵ�������һ�룬��ײ��ĵײ�������ĵײ���ƽ

    public static void VaryScaleButKeepChildLook(Transform shadowAt)
    {
        Transform bodyAt = shadowAt.GetChild(0);

        Vector2 sizeBody = GetSizeInScene(bodyAt.GetComponent<SpriteRenderer>().sprite);
        Vector2 sizeShadow = GetSizeInScene(shadowAt.GetComponent<SpriteRenderer>().sprite);//֪����żռ�����Ŀ�֪���Լ�ռ�����Ŀ�Ȼ��ɵõ�����
        float ratioWidth = sizeBody.x / sizeShadow.x;//��Ӱ��ȵ�����ż���
        float ratioHeight = sizeBody.y / 4 / sizeShadow.y;//��Ӱ�߶ȵ�����ż�߶��ķ�֮һ
        shadowAt.localScale = new Vector3(ratioWidth, ratioHeight);
        bodyAt.localScale = new Vector3(bodyAt.localScale.x / ratioWidth, bodyAt.localScale.y / ratioHeight, 1);//�����岻Ӧ���ܵ�������������Ӱ��

        //bodyAt.localPosition = new Vector3(0, sizeBody.y / 2 - sizeShadow.y / 2, 0);
        //����ż����ƫ�ƣ�ֱ���ײ�����Ӱ�ײ���ƽ�������ֵ����ż������᲻����Ԥ�ڣ�����������ַ�ʽ����
        bodyAt.localPosition = new Vector3(0, (sizeBody.y / 2 - sizeShadow.y / 2 * ratioHeight) / ratioHeight, 0);
        //ʵ����Ӱ�ĸ߶��Ѿ�������ԭͼƬ�ĸ߶��ˣ����Ի��ÿ����䱻���������

    }

    public static void VaryCollidToFitChildLook(Transform shadowAt)
    {
        //ǰ�����Ѿ�ѹ����shadow��ʹ����Ӱͼ��������ͼ�����λ�ú���
        //�ú����ᱣ����Ӱ������Ĺ�ϵ��Ȼ�󵥶��ı���ײ���С��ʹ�ʺ�����

        Transform bodyAt = shadowAt.GetChild(0);

        Vector2 sizeBody = GetSizeInScene(bodyAt.GetComponent<SpriteRenderer>().sprite);
        float ratioHeight = shadowAt.localScale.y;
        float ratioWidth = shadowAt.localScale.x;

        BoxCollider2D mCollid = shadowAt.gameObject.GetComponent<BoxCollider2D>();//��Ӱ��ײ��Ĵ�СӦ������ͬ��
        mCollid.size = new Vector2(sizeBody.x/ratioWidth, sizeBody.y / 2 / ratioHeight);//��Ӱ��ײ���������߶ȶ���֮һ
        //��scaleΪ1ʱ����ײ�ؿ�ĳ��ȵ�λ�ĳ���Ҳ����1����scale��Ϊ1ʱ���ؿ�ĵ�λ���Ȼᷢ���仯��transform.positionҲ�ᱻ�ı䵥λ����
        float bodyFootAt = shadowAt.position.y + bodyAt.localPosition.y * ratioHeight - sizeBody.y / 2;//��ż���λ���Ѿ������仯
        //����ż�ײ���λ�����ڣ�����ǰʵ��Ӧ����������
        //����ratioHeight��ԭ����y�ĵ�λ���ȱ�ѹ���ˣ�Ҫ��y�Ȼ�������Գ������֣���λ���Ⱥ�Ϊ1ʱ��ֵ�����õ���ǰ��Գ�������ʵ����
        float collidFootAt = shadowAt.position.y - mCollid.size.y / 2 * ratioHeight;
        mCollid.offset = new Vector2(0, (bodyFootAt - collidFootAt) / ratioHeight);
    }
}
