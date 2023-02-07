using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeepOnAbove : MonoBehaviour
{//��һ��������ʼ�����丸������Ϸ�(�·�ʱΪ��)����������ܱ��ؿռ�Ӱ��
 //�������������������

    public void SuSetHeightTo(float want)
    {
        mHeightAt = want > mHeightLowLimit ? want : mHeightLowLimit;
        ApplyHeight();
    }

    public void SuDeviateHeight(float vary)
    {
        SuSetHeightTo(mHeightAt + vary);
    }

    public void SuSetHeightOffset(float need)
    {
        mHeightOffset = need;
    }

    //=========================

    float mHeightLowLimit;
    float mHeightAtStart;
    float mHeightAt;//��Գ����Լ���������������

    float mHeightOffset;//����������Ӱ��

    public float meHeightStart { get { return mHeightAtStart; } }

    void Awake()
    {
        Vector3 posLocalStart = transform.localPosition;
        float scaleAcc = GbjAssist.GetSumScaleWhenParent(transform).y;
        mHeightAtStart = posLocalStart.y * scaleAcc;
        mHeightAt = mHeightAtStart < mHeightLowLimit ? mHeightLowLimit : mHeightAtStart;
    }

    void LateUpdate()
    {
        ApplyHeight();
    }

    void ApplyHeight() 
    {
        Vector3 origin = transform.parent.position;
        //������transform.position����תʱ��仯�����������е�x
        float posY = transform.parent.position.y + mHeightOffset + mHeightAt;
        transform.position = new Vector3(origin.x, posY, origin.z);
        //������λ�ÿ���ʱ�̶��ڱ仯
    }

}
