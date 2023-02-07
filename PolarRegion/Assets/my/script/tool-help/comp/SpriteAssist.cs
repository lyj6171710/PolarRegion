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

    public static void SetCollidBySprite(BoxCollider2D collid, SpriteRenderer render)//默认这两个组件位于同一物体上
    {
        Vector2 sizeBody = GetSizeInScene(render.sprite);
        collid.size = sizeBody;
    }

    //==========================================

    public static void MakeSibAlignToButtom(Transform sib,Transform self)
    {
        //假设self图像中心就是self的世界位置

        float scaleSelfY = self.localScale.y;
        float scaleSibY = sib.localScale.y;

        Vector2 sizeSib = GetSizeInScene(sib.GetComponent<SpriteRenderer>().sprite);
        Vector2 sizeSelf = GetSizeInScene(self.GetComponent<SpriteRenderer>().sprite);

        Vector3 localPos = sib.localPosition;
        sib.localPosition = new Vector3(localPos.x, 
            sizeSib.y / 2 * scaleSibY - sizeSelf.y / 2 * scaleSelfY, localPos.z);
        //假设已经底端对齐，然后计算sib相对self的偏移量
    }

    //=========================================

    //一个模板，父物体就表示个位置，有两个子物体，一个表示阴影，一个表示身体
    //最后让阴影等宽于身体，阴影高度是身体的四分之一，阴影位置与原来一致
    //身体大小与原来一致，身体会被阴影往上移动，直到身体底部与阴影底部齐平
    //阴影有一个碰撞体，要让碰撞体与身体等宽，宽度和身体一样，高度是身体一半，底部与身体底部齐平

    public struct Ifo
    {
        public float scaleShrinkX;
        public float scaleShrinkY;
    }

    public static void VaryScaleFitToSibLook(Transform pawn, Transform shadow, Ifo ifo)
    {
        //pawn表示身体，身体一定要和表示阴影的物体同级，同父

        Vector2 sizeBody = GetSizeInScene(pawn.GetComponent<SpriteRenderer>().sprite);
        Vector2 sizeShadow = GetSizeInScene(shadow.GetComponent<SpriteRenderer>().sprite);
        //知道人偶占场景的宽，知道自己占场景的宽，然后可得到比例
        float ratioWidth = sizeBody.x / ifo.scaleShrinkX / sizeShadow.x;//阴影宽度等于人偶宽度n分之一
        float ratioHeight = sizeBody.y / ifo.scaleShrinkY / sizeShadow.y;//阴影高度等于人偶高度n分之一
        shadow.localScale = new Vector3(ratioWidth, ratioHeight);//阴影其实不在父物体上，只是与脚底位置同步而已

        MakeSibAlignToButtom(pawn, shadow);
        //让人偶往上偏移，直至底部与阴影底部齐平
    }

    public static void VaryCollidToFitSibLook(Transform pawn, Transform shadow, Ifo ifo)
    {
        //foot表示父物体，不一定要是第一级父物体，所以不等同shadow.parent

        Vector2 sizeBody = GetSizeInScene(pawn.GetComponent<SpriteRenderer>().sprite);
        sizeBody *= GbjAssist.GetSumScaleWhenSelf(pawn);//实际情况
        float ratioWidth = shadow.localScale.x;
        float ratioHeight = shadow.localScale.y;

        BoxCollider2D collidShadow = shadow.GetComponent<BoxCollider2D>();
        //阴影的碰撞体用来检测人物行进的通路，不过碰撞体大小应与身体同步
        collidShadow.size = new Vector2(
            sizeBody.x / ifo.scaleShrinkX / ratioWidth, 
            sizeBody.y / ifo.scaleShrinkY / ratioHeight);//阴影碰撞体高度等于人偶高度的n分之一
        //当scale为1时，碰撞矩框的长度单位的长度也才是1，但scale不为1时，矩框的单位长度会发生变化
        float bodyFootAt = pawn.position.y  - sizeBody.y / 2;//人偶相对位置已经发生变化
        //求人偶底部的位置所在，面向当前实际应有情况来算的
        float collidFootAt = shadow.position.y - collidShadow.size.y / 2 * ratioHeight;
        collidShadow.offset = new Vector2(0, (bodyFootAt - collidFootAt) / ratioHeight);

    }

    //----------------------------------------------

    //一个模板，父物体是阴影，子物体是身体，最后让阴影等宽于身体，阴影高度是身体的四分之一，
    //阴影底部与身体底部齐平，身体大小与原来一致，阴影位置与原来一致
    //阴影同时具有一个碰撞体，要让碰撞体宽度等于身体，高度等于身体一半，碰撞体的底部与身体的底部齐平

    public static void VaryScaleButKeepChildLook(Transform shadowAt)
    {
        Transform bodyAt = shadowAt.GetChild(0);

        Vector2 sizeBody = GetSizeInScene(bodyAt.GetComponent<SpriteRenderer>().sprite);
        Vector2 sizeShadow = GetSizeInScene(shadowAt.GetComponent<SpriteRenderer>().sprite);//知道人偶占场景的宽，知道自己占场景的宽，然后可得到比例
        float ratioWidth = sizeBody.x / sizeShadow.x;//阴影宽度等于人偶宽度
        float ratioHeight = sizeBody.y / 4 / sizeShadow.y;//阴影高度等于人偶高度四分之一
        shadowAt.localScale = new Vector3(ratioWidth, ratioHeight);
        bodyAt.localScale = new Vector3(bodyAt.localScale.x / ratioWidth, bodyAt.localScale.y / ratioHeight, 1);//子物体不应该受到父物体伸缩的影响

        //bodyAt.localPosition = new Vector3(0, sizeBody.y / 2 - sizeShadow.y / 2, 0);
        //让人偶往上偏移，直至底部与阴影底部齐平，但这个值在人偶伸缩后会不符合预期，因此以上这种方式不行
        bodyAt.localPosition = new Vector3(0, (sizeBody.y / 2 - sizeShadow.y / 2 * ratioHeight) / ratioHeight, 0);
        //实际阴影的高度已经不再是原图片的高度了，所以还得考虑其被伸缩的情况

    }

    public static void VaryCollidToFitChildLook(Transform shadowAt)
    {
        //前提是已经压缩了shadow，使得阴影图像与身体图像比例位置合适
        //该函数会保持阴影与身体的关系，然后单独改变碰撞体大小，使适合需求

        Transform bodyAt = shadowAt.GetChild(0);

        Vector2 sizeBody = GetSizeInScene(bodyAt.GetComponent<SpriteRenderer>().sprite);
        float ratioHeight = shadowAt.localScale.y;
        float ratioWidth = shadowAt.localScale.x;

        BoxCollider2D mCollid = shadowAt.gameObject.GetComponent<BoxCollider2D>();//阴影碰撞体的大小应与身体同步
        mCollid.size = new Vector2(sizeBody.x/ratioWidth, sizeBody.y / 2 / ratioHeight);//阴影碰撞体等于身体高度二分之一
        //当scale为1时，碰撞矩框的长度单位的长度也才是1，但scale不为1时，矩框的单位长度会发生变化，transform.position也会被改变单位长度
        float bodyFootAt = shadowAt.position.y + bodyAt.localPosition.y * ratioHeight - sizeBody.y / 2;//人偶相对位置已经发生变化
        //求人偶底部的位置所在，面向当前实际应有情况来算的
        //乘以ratioHeight的原因是y的单位长度被压缩了，要让y等换到它相对场景这种，单位长度恒为1时的值，而得到当前相对场景的真实距离
        float collidFootAt = shadowAt.position.y - mCollid.size.y / 2 * ratioHeight;
        mCollid.offset = new Vector2(0, (bodyFootAt - collidFootAt) / ratioHeight);
    }
}
