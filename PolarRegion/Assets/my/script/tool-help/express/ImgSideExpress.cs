using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public interface IDirGet
{
    public float meAngleNow { get; }
}

public class ImgSideExpress : MonoBehaviour
{//ע��ͼ����ı��

    public Action meWhenFlip;

    public Sprite meProfile { get { return mRenderSelf.sprite; } }

    public bool meIsFaceToRight { get { return mStartFlip ? (-mCurSignFlip).ToBool() : mCurSignFlip.ToBool(); } }

    public float meOffsetToFoot { get 
        {
            if (!mConsiderDir)//Ĭ����Ϊ�����ױ��볡��ƽ�У�һֱ������
                return mOffsetToFoot;
            else
                return MathRect.CalcHighestPosY(mInclineHalf, mRadianToUR, mDir.meAngleNow);
        } 
    }

    //----------------------------------
    
    bool mStartFlip;//��ʼʱ�ķ�ת״̬��ͳһӦԤ��Ϊͼ�����ݳ���ʱ�ķ�ת״̬
    int mCurSignFlip;

    SpriteRenderer mRenderSelf;
    float mOffsetToFoot;
    public Vector2 mSize;

    bool mConsiderDir;//������תӰ��
    public float mInclineHalf;
    float mRadianToUR;
    IDirGet mDir;

    bool mHaveReady;

    public float meFootAt { get { return transform.position.y - mOffsetToFoot; } }

    public void MakeReady(SpriteRenderer diagramRefer)
    {
        mRenderSelf = GbjAssist.AddCompSafe<SpriteRenderer>(gameObject);
        //��ͬʱ��Ӧ�ο��߾����Լ������
        if (diagramRefer != null)
        {//�������գ�˵������Ҫ������������趨��
            mRenderSelf.sprite = diagramRefer.sprite;
            mStartFlip = diagramRefer.flipX;
        }
        else
        {
            mStartFlip = mRenderSelf.flipX;
        }
        meWhenFlip = () => { };
        mRenderSelf.flipX = false;
        //��Ⱦ�����Ԥ�õ�flip״ֻ̬������������������յ���scale��ʵ������
        SuSwitchFace(mStartFlip ? -1 : 1);

        mHaveReady = true;
    }

    public void ReadyCountSize()
    {//�����������������������ܷ����ڸ����׼���֮��
     //���ȷ�����ᷢ�����������������ø÷���������һ����ʼ�������
        Vector2 scaleAcc = GbjAssist.GetSumScaleWhenSelf(transform);
        mSize = SpriteAssist.GetSizeInScene(mRenderSelf.sprite) * scaleAcc;
        mOffsetToFoot = mSize.y / 2;//����ŵ�������ĵĳ���ƫ����
    }

    public void ReadyConsiderDir()
    {
        //����ǰ���Ѿ������п��Ʒ�������
        mConsiderDir = true;

        //��������ڳ���y�᷽���ϵ����ƫ�Ƶ�
        mDir = gameObject.GetComponent<IDirGet>();
        mInclineHalf = MathRect.CalcRectInclineHalf(mSize);
        mRadianToUR = MathRect.CalcRadianToUR(mSize);
    }

    public void SuSwitchFace(float faceTo = 0)
    {//�������ֵ����0����֤ͼ��������ǳ��ҵ�
        if (faceTo != 0)
        {
            Vector3 origin = transform.localScale;
            origin.x = MathNum.Abs(origin.x);
            int lastSign = mCurSignFlip;
            if (faceTo > 0)
                mCurSignFlip = mStartFlip ? -1 : 1;
            //���һ��ʼ���ǳ��ҵģ���ôһ��ʼ��Ҫ�ķ�ת����false
            else //faceTo < 0
                mCurSignFlip = mStartFlip ? 1 : -1;
            if (lastSign != mCurSignFlip)
            {
                transform.localScale = new Vector3(mCurSignFlip * origin.x, origin.y, origin.z);//ת��ĸı䣬��ֻ��ͼ����������������ײ��
                meWhenFlip();
            }
        }
    }

    public void SuSetShowLayer(int need)
    {
        mRenderSelf.sortingOrder = need;
    }

}
