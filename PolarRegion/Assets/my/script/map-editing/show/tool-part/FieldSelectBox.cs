using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class IfoSelectBox
{
    public Sprite border;
    public float scale;
}

public class FieldSelectBox : MonoBehaviour
{
    //���������ͨ�ã���Ҫ�����������趨���ܣ��������Ծֲ���������

    //û�и��û��ƣ�һ�����˳��أ���þ�ɾ���ؽ�һ��

    public Service me;

    Vector2Int mCurHover;//���һ����������ڵ�����
    Vector2Int mCurSelect;
    bool mHoverInField;//��ǰ��ѡ������
    bool mInMultiSelect;//��ѡѡ��״̬
    Vector2Int mSelectOffset;//֧�ֶ�ѡ

    public Action SuWhenNewSelect;
    public Action SuWhenMultiSelectEnd;
    public Action SuWhenSingleSelectEnd;

    //---------------------------

    float mDepth;
    Vector3 mScaleStart;

    IfoSelectBox mIfo;

    FieldGridMager mSelectIn;

    void Update()
    {
        mHoverInField = mSelectIn.SuWhetherCursorIn(mCurHover);

        if (me.CanSelect)
        {
            if (UnifiedInput.It.meInConfirm())
            {
                if (!mInMultiSelect)//�տ�ʼ����ѡ��״̬
                {
                    if (mHoverInField) 
                    {
                        mSelectOffset = Vector2Int.zero;
                        mInMultiSelect = true;//��Ӱ�죬�������µ�Ԫ��ʱ�ģ��ж�����

                        if (mCurSelect != mCurHover)
                        {
                            mCurSelect = mCurHover;
                            ShowUpdateRange(mCurSelect);
                            SuWhenNewSelect();
                        }
                    }
                }
            }
            else
            {
                if (mInMultiSelect)
                {
                    if (mSelectOffset != Vector2Int.zero)
                        SuWhenMultiSelectEnd();
                    else 
                        SuWhenSingleSelectEnd();
                    mInMultiSelect = false;
                }
            }
        }
        else
        {
            mInMultiSelect = false;
        }
    }

    //===================================

    public void MakeReady(FieldGridMager targetIn, IfoSelectBox ifo)
    {
        mSelectIn = targetIn;
        mIfo = ifo;

        Vector2Int oneAt;
        GameObject oneGrid = mSelectIn.GetAnyOne(out oneAt);
        mDepth = oneGrid.transform.position.z - 0.1f;//������ʾ
        transform.position = new Vector3(0, 0, mDepth);
        mScaleStart = transform.localScale *= ifo.scale;
        SpriteRenderer render = gameObject.AddComponent<SpriteRenderer>();
        render.sprite = mIfo.border;
        render.sortingOrder = GlobalConfig.prior_ui;

        mSelectIn.meWhenHoverOneGridNew += (coord) =>
        {
            mCurHover = coord;

            if (mInMultiSelect)
            {
                mSelectOffset = coord - mCurSelect;//�߼�����ϵ�ϵ�ƫ��
                ShowUpdateRange(CurSelectUL(), CurSelectExtend());
            }
        };

        ShowUpdateRange(oneAt);
        SuShowBox(false);

        SuWhenNewSelect += () => { };
        SuWhenSingleSelectEnd += () => { };
        SuWhenMultiSelectEnd += () => { };

        me = new Service(this);
    }

    public void SuUpdateByMager()//����������״̬�����¶�λ
    {
        ShowUpdateRange(CurSelectUL(), CurSelectExtend());
    }

    public void SuShowBox(bool onoff)
    {
        if (onoff)
            gameObject.SetActive(true);
        else
            gameObject.SetActive(false);
    }

    //------------------------------------

    void ShowUpdateRange(Vector2Int posStart, Vector2Int extend = default(Vector2Int))
    {
        //���߼�����ϵ�ϣ���һ����㣬Ȼ����x��y��������������
        //Ҫ��extendֻ������Ȼ��

        //ֻ�Ǳ�����ʾ�ϵĿ��ƣ���Ӱ���ڲ�ѡ��״̬

        float xStart = mSelectIn.SuGetPos(posStart).x;
        float yStart = mSelectIn.SuGetPos(posStart).y;

        Vector2 end = mSelectIn.SuGetPos(posStart + extend);
        float xEnd = end.x;
        float yEnd = end.y;

        transform.position = new Vector3((xStart + xEnd) / 2, (yStart + yEnd) / 2, mDepth);
        transform.localScale = mScaleStart * new Vector2(extend.x + 1, extend.y + 1);
    }

    Vector2Int CurSelectUL()//����ѡ�����ݣ�����ѡ��������ʾ������
    {
        Vector2Int startAt = mCurSelect;
        Vector2Int offsetTo = startAt + mSelectOffset;

        int x, y;
        if (mSelectOffset.x >= 0) x = startAt.x;
        else x = offsetTo.x;
        if (mSelectOffset.y >= 0) y = startAt.y;
        else y = offsetTo.y;

        return new Vector2Int(x, y);
    }

    Vector2Int CurSelectExtend()
    {
        int xExtend = mSelectOffset.x >= 0 ? mSelectOffset.x : -mSelectOffset.x;
        int yExtend = mSelectOffset.y >= 0 ? mSelectOffset.y : -mSelectOffset.y;
        return new Vector2Int(xExtend, yExtend);
    }

    //===========================

    public class Service : Srv<FieldSelectBox>
    {
        public Service(FieldSelectBox belong) : base(belong)
        {
        }

        public Vector2Int CurHover => j.mCurHover;
        public Vector2Int CurSelect => j.mCurSelect;
        public bool IsHoverSelect { get { return j.mSelectIn.SuWhetherCursorIn(j.mCurSelect); } }
        public bool InMultiSelect { get { if (j.mSelectOffset.magnitude == 0) return false; else return true; } }
        public bool HoverInField => j.mHoverInField;

        public bool CanSelect;//��������ѡ��

        public Vector2Int[][] CurSelects//��簴ѡ���ϵı�����ȡֵ
        {
            get
            {
                Vector2Int selectRect = j.CurSelectExtend() + new Vector2Int(1, 1);
                Vector2Int[][] selects = new Vector2Int[selectRect.x][];
                for (int i = 0; i < selectRect.x; i++)
                {
                    selects[i] = new Vector2Int[selectRect.y];
                    for (int p = 0; p < selectRect.y; p++)
                    {
                        selects[i][p] = j.CurSelectUL() + new Vector2Int(i, p);
                    }
                }
                return selects;
            }
        }
    }

}
