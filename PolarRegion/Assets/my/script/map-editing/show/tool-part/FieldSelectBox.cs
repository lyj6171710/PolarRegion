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
    //面向坐标才通用，不要面向序数来设定功能，不过可以局部借助序数

    //没有复用机制，一旦换了场地，最好就删了重建一个

    public Service me;

    Vector2Int mCurHover;//最后一次鼠标悬浮在的区域
    Vector2Int mCurSelect;
    bool mHoverInField;//当前在选区内吗
    bool mInMultiSelect;//多选选择状态
    Vector2Int mSelectOffset;//支持多选

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
                if (!mInMultiSelect)//刚开始进入选择状态
                {
                    if (mHoverInField) 
                    {
                        mSelectOffset = Vector2Int.zero;
                        mInMultiSelect = true;//会影响，悬浮到新单元格时的，行动流程

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
        mDepth = oneGrid.transform.position.z - 0.1f;//优先显示
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
                mSelectOffset = coord - mCurSelect;//逻辑坐标系上的偏移
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

    public void SuUpdateByMager()//根据网格新状态，重新定位
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
        //在逻辑坐标系上，给一个起点，然后往x、y增长方向上扩增
        //要求extend只能是自然数

        //只是表面显示上的控制，不影响内部选择状态

        float xStart = mSelectIn.SuGetPos(posStart).x;
        float yStart = mSelectIn.SuGetPos(posStart).y;

        Vector2 end = mSelectIn.SuGetPos(posStart + extend);
        float xEnd = end.x;
        float yEnd = end.y;

        transform.position = new Vector3((xStart + xEnd) / 2, (yStart + yEnd) / 2, mDepth);
        transform.localScale = mScaleStart * new Vector2(extend.x + 1, extend.y + 1);
    }

    Vector2Int CurSelectUL()//根据选择数据，给出选框用来显示的数据
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

        public bool CanSelect;//用来保持选择？

        public Vector2Int[][] CurSelects//外界按选择上的表现来取值
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
