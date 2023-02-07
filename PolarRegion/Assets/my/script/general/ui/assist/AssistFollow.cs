using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AssistFollow : MonoBehaviour////����ǰ������Ԥ�����ϣ�Ȼ���������������𤸽Ч��
{//���ǻ��࣬����ֱ��ʹ�ã���Ҫʹ�ü̳и���������
 //���ظ���������壬���ù�����AssistPos�����

    //���ɲ���=================================

    public bool mFollow;
    public Mode mMode = Mode.point;

    public virtual void BackToStart(){ }

    //˽�ñ���==================================

    bool mHaveReady = false;
    protected AssistPos mPos;//���ṩһϵ��λ�����ݵİ���
    protected RectTransform meSelf { get { return mPos.meSelfLay; } }
    protected Vector2 mNeed;//����ȥ����������������������ģʽ�Ĳ�ͬ����ͬ
    protected bool mForce;//�����л������һ��ǿ��λ�õ���

    void Update()
    {
        UpdateNer();
        if (mFollow)
        {
            UpdatePos();
        }
        else if (mForce)
        {
            UpdatePos();
            mForce = false;//�Զ��ر�
        }
    }

    void UpdatePos()//���ݵ�ǰ���ݣ����±���
    {
        if (mMode == Mode.point)
        {
            //��ʱmNeed����ֵ��λ��UI�����ĳ��ȵ�λ
            meSelf.localPosition = mNeed;
        }
        else
        {
            meSelf.localPosition += new Vector3(mNeed.x, mNeed.y, 0);
            mNeed = Vector2.zero;
        }
    }


    //�ܹ���Ҫ========================================

    protected virtual void UpdateNer(){ }

    protected virtual void StartNer(){ }

    void Start()
    {
        if (mHaveReady) return;
        mPos = GetComponent<AssistPos>();
        mPos.MakeReady();
        mForce = false;
        StartNer();
        mHaveReady = true;
    }

    public enum Mode { point,offset }//����ģʽ
}
