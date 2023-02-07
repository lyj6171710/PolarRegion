using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EKindInput { none, keyboard, mouse, touch }

public partial class UnifiedInput : MonoBehaviour,ISwitchScene
{
    //����ͳһ�������͵���ҡ��������
    //�ײ�����ĸı�ǣ��ͳһ����ĸı䣬�������ͳһ�����״̬�����ж���Ӧ

    //Ĭ����Ҫ�����̼�������ͳһ����ͻ�ȡ����Ӧ����״̬������������˵��

    //�����Ҫ��update���������룬��Ȼ�����������Ƴ�ͻ����ɲ���֪�Ľ��

    public bool meInConfine => mAreaAsker != null;
    
    public void NormalizeArea(object asker)
    {
        if (asker != null && mAreaAsker == asker)
        {
            //��Ȩ�������
            RectMeter rect = new RectMeter();
            rect.leftBottom = Vector2.zero;
            rect.rightTop = SceneViewL.It.SuGetSizeViewCur();
            mAreaSure = rect;
            mAreaAsker = null;
        }
    }

    public void ConfineArea(RectMeter rect, object asker)
    {
        if (asker == null)
            return;//��Ȩ����
        else if (mAreaAsker == null)
        {
            mAreaSure = rect;
            mAreaAsker = asker;
        }
        else if (mAreaAsker == asker)
            mAreaSure = rect;//�������µ���
    }

    bool WhetherBanAnswerAction(object inquirer)
    {
        //���Բ���ѯ���ߣ��ᰴ��С���޴���
        //������ʱ�����ֻ��ʹ������ӿڲ��ܶ���Ϊ��������и�Ӧ��
        if (mAreaAsker == null) return false;
        else if (mAreaAsker == inquirer) return false;
        else return true;
    }//�Ƿ���Զ����Ӧ��ǰ������������Ϊ
    //ʵ�ֽ���ǰ����ר����ĳλ�����������ｫ�޷��Ӹ������Ӧ��������Ϊ�ķ���

    RectMeter mAreaSure;//�Ӿ����������
    //������Ļ�ռ�����(�ٷֱ�)������״̬�²���Ч
    bool mInLock;//���Ӿ�������������ֿ���������
    object mAreaAsker;//���޶����򵽲��޶�����Ϊһ�����ɷֽ׶�

    //======================================

    public EToward4 meGoOneStep(object inquirer = null){ 
        if (WhetherBanAnswerAction(inquirer)) return EToward4.middle;
        for (int i = 0; i < 4; i++)
        {
            if (mTowards[i])
                return MapGo(i);
        }
        return EToward4.middle;
    }

    public bool SuIfKeepGoing(EToward4 to, object inquirer = null)
    {
        if (WhetherBanAnswerAction(inquirer)) return false;
        return mShifts[MapGo(to)];
    }

    public EToward4 meInGo(object inquirer = null)
    {
        if (WhetherBanAnswerAction(inquirer)) return EToward4.middle;
        for (int i = 0; i < 4; i++)
        {
            if (mShifts[i])
                return MapGo(i);
        }
        return EToward4.middle;
    }

    bool[] mShifts;//�ж�ȥ�������̼�ʱ��һֱΪtrue
    bool[] mShiftWaits;//���������ж�״̬

    bool[] mTowards;//�ж�����һ���ԣ������̼�ʱ������һֱΪtrue
    float[] mGoGaps;//��Ȼ����һֱΪtrue������һ��ʱ����������ٴα�����
    const float cGoGap = 0.33f;
    const int cTowardNum = 4;

    public void ExciteGo(EToward4 go)
    {
        ProExciteGo(MapGo(go));
    }

    void ProExciteGo(int which)
    {
        mShiftWaits[which] = true;
        if (mGoGaps[which] < 0)
            mGoGaps[which] = cGoGap;
    }

    void AwakeToward()
    {
        mShifts = new bool[cTowardNum];//��������
        mShiftWaits = new bool[cTowardNum];

        mTowards = new bool[cTowardNum];
        mGoGaps = new float[cTowardNum];
    }

    void LateUpdateToward()
    {
        for (int i = 0; i < cTowardNum; i++)
        {
            if (OverAssist.OnThenOff(ref mShifts[i], ref mShiftWaits[i]))
            {
                if (mGoGaps[i] == cGoGap)//��ʼ�����̼��������̼�һ��ʱ���
                    mTowards[i] = true;
                else
                    mTowards[i] = false;
                mGoGaps[i] -= Time.deltaTime;
            }
            else//�������true��Ϊfalseʱ
            {
                mTowards[i] = false;
                mGoGaps[i] = -1;//���ֹͣ�����̼�����ô���ϾͿ����ٴ���
            }
        }
    }

    int MapGo(EToward4 to)
    {
        switch (to)
        {
            case EToward4.up: return 0;
            case EToward4.down: return 1;
            case EToward4.left: return 2;
            case EToward4.right: return 3;
            default: return -1;
        }
    }

    EToward4 MapGo(int index)
    {
        switch (index)
        {
            case 0: return EToward4.up;
            case 1: return EToward4.down;
            case 2: return EToward4.left;
            case 3: return EToward4.right;
            default: return EToward4.middle;
        }
    }

    //======================================

    public bool meIsMoving(object inquirer = null) => mInMove && !WhetherBanAnswerAction(inquirer);
    public Vector2 meMoveDir => mMoveDir;

    Vector2 mMoveDir;
    bool mInMove;
    bool mMoveWait;

    public void ExciteMove(Vector2 to)
    {
        if (!MathNum.IsNear0(to.magnitude))//��������ϵͳ�ģ�ϵͳ�ڲ������Լ������
        {
            mMoveDir = to.normalized;
            mMoveWait = true;
        }
    }

    void LateUpdateMove()
    {
        if (!OverAssist.OnThenOff(ref mInMove, ref mMoveWait))
            mMoveDir = Vector2.zero;
    }

    //==================================

    public bool SuTapSureUdCfn(object inquirer)
    {
        //��Ҫ��֤��ݣ���Ϊ����ͬʱ���ڶ����������������
        if (inquirer == mAreaAsker)
            return mTapSure;
        else
            return false;
    }

    public bool meInConfirm(object inquirer = null) 
        => mInSure && !WhetherBanAnswerAction(inquirer);
    public bool meInConfirmJust(object inquirer = null) 
        => mInSureJust && !WhetherBanAnswerAction(inquirer);//���г�����Ӧ����ʱ������
    public bool meTapConfirm(object inquirer = null) 
        => mTapSure && !WhetherBanAnswerAction(inquirer);
    public bool meLongConfirm(object inquirer = null) 
        => ((mInSure && mInSureTime >= cTapLimit) ? true : false) && !WhetherBanAnswerAction(inquirer);
    public bool meDoubleConfirm(object inquirer = null) 
        => mDoubleSure && !WhetherBanAnswerAction(inquirer);

    //public bool meReleaseConfirm => mReleaseSure;//�ͼ�ֵ�ӿ�

    bool mInSure;//���£�ֱ��release
    bool mInSureJust;//���º�ĺ�һ֡Ϊtrue��ֱ����һ�ΰ��²Ż��ٴ�Ϊtrue
    bool mReleaseSure;
    bool mTapSure;//�����ֶ�ʱ�����ɿ�
    bool mDoubleSure;

    const float cTapLimit = 0.3f;
    float mInSureTime;//���µ�ʱ������

    bool mReleaseSureWait;//�ӳ�һ֡��ص�״̬
    bool mTapSureWait;//ͬ��
    bool mInSureJustWait;
    bool mDoubleSureWait;

    EKindInput mSureBind;
    Vector2 mSurePosStart;
    IClick mSureRespond;//�¼��������Լ������

    public void ExciteStartSure(EKindInput eKind, bool isCursor)
    {
        if (!mInSure)
        {
            if (meInConfine && isCursor)
            {
                Vector2 percentAt = CoordFrame.
                    SuCoordPercentInScreenFromMeter(UnifiedCursor.It.meMeterAt);
                if (!MathRect.SuWhetherInside(percentAt, mAreaSure, 0))
                    return;//�ɹ��ȷ�ϣ������������ڣ����ܼ���ȷ������
            }

            if (IsAvailable(ref mSureBind, eKind)) 
            {
                mInSureTime = 0;
                mInSureJustWait = true;//ִ�к����һ֡��Ч
            }
        }
    }
    //����Ҫ�����̼���ͨ���̼�release����ʾ����
    //Ĭ��������release�ĺ�һ֡�����ſ��ܻ�ִ�иú�������Ȼû����

    public void ExciteReleaseSure(EKindInput eKind)
    {
        if (!mReleaseSure)
        {
            if (mSureBind != EKindInput.none && eKind == mSureBind) 
            {
                mReleaseSureWait = true;
            }
        }
    }
    //����Ҫ�����̼�
    //Ĭ�������ڿ�ʼsure�ĺ�һ֡�����ſ��ܻ�ִ�иú�������Ȼû����
    //ִ�к����һ֡��Ч

    void UpdateSure()
    {
        if (mInSure)
        {
            mInSureTime += Time.deltaTime;
        }
    }

    void LateUpdateSure()
    {
        if (OverAssist.OnThenOff(ref mInSureJust, ref mInSureJustWait))
        {
            mInSure = true;
            mSurePosStart = UnifiedCursor.It.meMeterAt;
            mSureRespond = CoordUse.SuCheckHit(UnifiedCursor.It.meMeterAt);
            if (mSureRespond != null) mSureRespond.WhenDown();

            if (TimeCount.It.SuMakeClickIfTwo("UnInSure", cTapLimit))
                mDoubleSureWait = true;//˫��ʱ�͵ڶ��ε�����ͬһ֡��Ӧͬ���俪��״̬
        }

        OverAssist.OnThenOff(ref mDoubleSure, ref mDoubleSureWait);

        if (OverAssist.OnThenOff(ref mReleaseSure, ref mReleaseSureWait))
        {
            if (mInSureTime < cTapLimit)//����ʱ������Ϊ����һ�ε��
            {
                mTapSureWait = true;
            }
            mInSure = false;
            mSureBind = EKindInput.none;

            if (mSureRespond != null)
            {
                mSureRespond.WhenUp();
            }
        }

        if (OverAssist.OnThenOff(ref mTapSure, ref mTapSureWait))
        {
            if (mSureRespond != null)
            {
                if ((mSurePosStart - UnifiedCursor.It.meMeterAt).magnitude < 10) //һ��ת��λ���ˣ�Ҳ������Ϊ�ǵ����Ϊ
                    mSureRespond.WhenHit();
            }
        }
    }

    //====================================

    public bool meWhenBack(object inquirer = null)
        => mBack && !WhetherBanAnswerAction(inquirer);

    bool mBack;
    bool mBackWait;

    bool mBackWant;
    float mBackTime;//�̼���ͣ��ʱ����ӣ�����ʱ��Ϊȡ��
    int mBackDown;//����ʱ
    const float cBackLimit = 0.3f;

    public void ExciteBack()//���������̼���������Ч
    {
        if (!mBack)
        {
            mBackWant = true;
            mBackDown = 2;//Ӧ������Ҫ�ɿ�
        }
    }

    void UpdateBack()
    {
        if (mBackWant)
        {
            mBackDown -= 1;
            mBackTime += Time.deltaTime;
            if (mBackDown == 0)
            {
                if (mBackTime <= cBackLimit) 
                {
                    mBackWait = true;
                }
                mBackTime = 0;
                mBackWant = false;
            }
        }
    }

    void LateUpdateBack()
    {
        OverAssist.OnThenOff(ref mBack,ref mBackWait);
    }

    //==================================

    public int meNumInput => mNumInput;

    int mNumInput;

    public void ExciteNum(int num)
    {
        mNumInput = num;
    }

    //==================================

    Dictionary<string, InfoKeyPress> mKeys;

    public bool SuWhenTap(string key, object inquirer = null)
    {
        if (WhetherBanAnswerAction(inquirer)) return false;
        if (mKeys.ContainsKey(key))
            return mKeys[key].pressTap;
        else
            return false;
    }
    
    public bool SuWhenInPressJust(string key, object inquirer = null)//���̰ܶ����ǳ�������ִֻ��һ��
    {
        if (WhetherBanAnswerAction(inquirer)) return false;
        if (mKeys.ContainsKey(key))
            return mKeys[key].pressJust;
        else
            return false;
    }

    public bool SuWhenInPress(string key, object inquirer = null)//ֻҪ���ţ���һֱִ��
    {
        if (WhetherBanAnswerAction(inquirer)) return false;
        if (mKeys.ContainsKey(key))
            return mKeys[key].inPress;
        else
            return false;
    }

    public void ExciteKey(string key)
    {
        if (mKeys.ContainsKey(key))
        {
            mKeys[key].pressWait = true;
        }
        else
        {
            InfoKeyPress keyPress = new InfoKeyPress();
            keyPress.pressWait = true;
            mKeys.Add(key, keyPress);
        }
    }

    void AwakeKey()
    {
        mKeys = new Dictionary<string, InfoKeyPress>();
    }

    void LateUpdateKey()
    {
        foreach (string key in mKeys.Keys)
        {
            InfoKeyPress state = mKeys[key];
            
            if (OverAssist.OnThenOff(ref state.pressJust, ref state.pressWait))
            {
                if (state.releaseJust)//�ɿ�֡�ĺ�һ֡����Ӧ�ùص����ɿ�״̬��
                {
                    state.pressTap = false;
                    state.releaseJust = false;
                }

                if (state.inPress)//�жϸհ�״̬����һ�ε���ʱ������
                    state.pressJust = false;//��һLateUpdateǰ��false״̬
                else
                    state.inPress = true;//���밴ס״̬

                state.pressTime += Time.deltaTime;
            }
            else//����״̬����ʱ����һ֡��ȷʵ�ɿ���
            {
                if (state.inPress)
                {
                    if (state.pressTime < 0.33f)//�ж��Ƿ����ڶ̰�
                        state.pressTap = true;

                    state.pressTime = 0;
                    state.inPress = false;
                    state.releaseJust = true;
                }
                else//�ɿ���һ֡����һ֡����ʱ
                {
                    if (state.releaseJust)
                    {
                        state.releaseJust = false;
                        state.pressTap = false;
                    }
                }
            }
        }
    }

    //====================================

    public static bool IsAvailable(ref EKindInput bind, EKindInput apply)
    {
        if (bind == EKindInput.none)
        {
            bind = apply;
            return true;
        }
        else if (bind == apply)
        {
            return true;
        }
        else return false;
    }//��������ʱ�������뷽ʽ������

    //=================================

    void Awake()
    {
        AwakeToward();
        AwakeKey();
    }

    void Update()
    {
        UpdateSure();
        UpdateBack();
    }

    void LateUpdate()
    {
        LateUpdateBack();
        LateUpdateSure();
        LateUpdateToward();
        LateUpdateMove();
        LateUpdateKey();
    }

    //=================================

    public static UnifiedInput It;

    public void WhenAwake()
    {
        It = this;

        mNumInput = 1;
    }

    public void WhenSwitchScene()
    {

    }
}

public class InfoKeyPress
{
    public bool pressWait;//��ǰ�Ƿ񼤻��˰���
    public bool pressJust;//�Ƿ�ղŲŰ�
    public bool pressTap;//�ǳ����İ�
    public bool inPress;//�Ƿ�����ס
    public float pressTime;//��ס��ʱ��
    public bool releaseJust;//�ղ��ɿ�
}

public interface IClick
{
    void WhenDown();
    void WhenUp();
    void WhenHit();
}
