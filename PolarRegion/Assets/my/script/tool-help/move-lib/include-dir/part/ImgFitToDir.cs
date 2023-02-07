using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImgFitToDir : MonoBehaviour, IDirGet
{//��ͼƬ����ͳ���ʱ�����������ͼƬ��������ķ���
 //����Գ����仯�������ӱ��ؿռ䣬���Ҫ���ǣ��������Լ����뱾�����غ��ٽ����ֵ���ݸ������
 //������а����������ת״̬����粻Ҫ���Լ��޸�Ӱ����ת������

    public Vector2 meDirAim;//͹��Ŀ���ԣ�����˲����Ӧ��
    public float meAngleAim;

    public float meAngleNow { get { return mCurRightTo; } }

    public void SuFitInstantly()//�޽���Ч����˲�䵽��ָ������
    {
        mCurRightTo = GetDirNeed();//ֱ��Ŀ��ֵ
        FitToNewDir(mCurRightTo);
    }

    public void SuAutoFit(bool onoff)//�Զ����������õ�Ŀ��ֵ
    {
        if (!mHaveReady) return;
        if (onoff)
        {
            mCurRightTo = MathAngle.GetAngle(transform.right) + mAngleSelf;//��ȡ�����ڵ�ǰ����
            mInUse = true;
        }
        else
            mInUse = false;
    }

    //====================================

    float mAngleSelf;//���ͼ������ָ���ǳ��ҵģ���������ͼ��ָ��������ҵ���ʱ�����
    //�������Ŀ���ǣ�ʹ��ͼ���ں���������˳Ӧ��ָ���ķ���
    bool mHaveReady;
    EAngle mFormUse;

    bool mStartMirror;
    int mStartMirrorSelf;
    bool mMirrorIgnore;//���Ǿ����Ӱ����������ܸ��������Ҫ����ѡ���Կ���

    float mCurRightTo;//�߼��ϣ���ǰͼ������ָ����Գ��ҵķ��򣨳��ң���������ϵ�����ָ��
    bool mInUse;//����Ч��

    public void MakeReady(float angleSelf, EAngle form)
    {
        if (mHaveReady) return;

        mFormUse = form;
        mAngleSelf = angleSelf;

        mCurRightTo = mAngleSelf;//ͼ���Ҳ൱ǰָ������

        ConsiderMirror();

        mHaveReady = true;
    }

    void ConsiderMirror()
    {//��һ��һ��ʼ�ͻῼ�ǣ���綨�¸���������������transform״̬���ٵ���?
     //��Ӱ��ģ����Ǿ��񣬽����ǲ����ܾ���Ӱ�죬���˷����Ҿ��ܸ���ת����ȥ
        mMirrorIgnore = false;
        mStartMirror = GbjAssist.GetSumScaleWhenParent(transform).x > 0 ? true : false;
        mStartMirrorSelf = transform.localScale.x > 0 ? 1 : -1;
    }

    void LateUpdate()
    {
        if (!mHaveReady) return;

        if (mInUse)
        {
            float needDir = GetDirNeed();
            mCurRightTo = MathNum.Lerp(mCurRightTo, needDir, 0.25f);
            FitToNewDir(mCurRightTo);
        }
        else
        {
            SuFitInstantly();//��ʱӦ�ã�������������б䶯
        }

    }

    void FitToNewDir(float angleNeed)
    {
        SetRightTo(MathAngle.AngleToVector(angleNeed - mAngleSelf));
        //ͼ���Ҳ���Գ��ҷ��� = ������Գ��ҵķ��� + Դͼ����Գ��ҵķ���

        GetRidOfMirror();
    }

    void GetRidOfMirror()
    {
        //������Գ�����ά��ԭ���ľ���״̬
        //��Ҫÿʱ��ִ�У���Ϊ�����ʱ�����ܸĶ�����ֵ��ǣ���͸ı��˷���
        if (mMirrorIgnore) return;
        Vector3 origin = transform.localScale;
        if (mStartMirror != GbjAssist.GetSumScaleWhenParent(transform).x > 0)
        {
            transform.localScale = new Vector3(Mathf.Abs(origin.x) * -1 * mStartMirrorSelf, origin.y, origin.z);
        }
        else
        {
            transform.localScale = new Vector3(Mathf.Abs(origin.x) * mStartMirrorSelf, origin.y, origin.z);
        }
        //�����徵�񻯺���������Ҫ�ص�ԭ��״̬����Ҫ���������ͬʱ����ת�Ƕȱ为��
        //��ת�Ƕȱ为����������ͼ���Ҳ�ָ��ԭ����ָ�ķ��������ά���Ҳ෽����ô����Ҫ�ֶ��ø�����
    }

    float GetDirNeed()//ʹ�ö�����﷽�򣬸�����⣬Ҳ������ʵ�ֲ�ֵ
    {
        if (mFormUse == EAngle.vector)
            return MathAngle.GetAngle(meDirAim);
        else if (mFormUse == EAngle.degree)
            return meAngleAim;
        else 
            return 0;
    }

    void SetRightTo(Vector2 dir)//��û�п���ͼ�����״̬����Ҫ�Ѿ������ڲ�����
    {
        if (Vector2.Dot(dir, Vector2.left) < 0.9995f) //������ת��ʽ������֧������������Ҫ��ת��ǰͣ��
            transform.right = dir;//transform.right����Գ�����
                                  //����ʱ��Գ�����������������ܻᱻ�����ı��
        else
        {
            if (dir.y >= 0)
                transform.right = new Vector2(-1, 0.001f);
            else
                transform.right = new Vector2(-1, -0.001f);
        }
        //ʹ������һ��������ʱ�������ܱ�֤�������������໹���Ҳࣨ����unity���ƣ���������һֱ���������࣬����Ͳ��ù˼���
    }

    //============================
    //�������֪ʶ��
    //localEulerAngles.x������Ǳ༭��transform��ʾ���е���תֵ����rotation.x����Ԫ����һ��ά��
}
