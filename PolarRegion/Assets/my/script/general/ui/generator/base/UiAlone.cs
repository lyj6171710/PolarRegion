using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public sealed class UiAlone : MonoBehaviour
{
    //该组件主要帮助以及负责定位
    //该组件应挂载于单个整体中的总物体上
    //该组件需处于overlay模式的画布下

    //组合可用==========================

    //一次性赋值========================

    Ifo mPos;//info-pos 
    IfoFollow mFollow;
                     
    //自用数据==========================

    bool mIsReady = false;
    AssistPos mPosCtrl;
    RectTransform mRectTransform;
    
    void Update()
    {
        if (mFollow.inNeed)
            if (mFollow.to)
            {
                if (mPos.byWorldOrCanvas)
                {
                    mPosCtrl.SuUpdatePosByWorld(mFollow.to.position);//等同化
                    ShiftToPos(false, (Vector2)mRectTransform.localPosition + mPos.posOffset);
                }
                else
                {
                    if (mPos.byGlobalOrLocal)
                        ShiftToPos(true, new Vector2(mRectTransform.position.x, mRectTransform.position.y) + mPos.posOffset);
                    else
                        ShiftToPos(false, new Vector2(mFollow.to.localPosition.x, mFollow.to.localPosition.y) + mPos.posOffset);
                }
            }

    }

    public void ShiftToPos(bool globalOrLocal, Vector2 pos)//参数是相对画布的绝对位置
    {
        if (globalOrLocal)//基层直接对应相等，应付不利一致的情况
            mRectTransform.position = new Vector3(pos.x, pos.y, mRectTransform.position.z);
        else//这个想要效果对，申请方的各父级，其轴心也都得在画布中心
            mRectTransform.localPosition = pos;
    }

    //内外机制================================
    
    public void AcceptToLocate(Ifo need)
    {
        mPos = need;
    }

    public void MakeReady()//需要在接收参数后，被外界紧接调用
    {
        if (!mIsReady)
        {
            mRectTransform = GetComponent<RectTransform>();
            mPosCtrl = gameObject.AddComponent<AssistPos>();
            mPosCtrl.MakeReady();
            //默认该组件挂载在的物体，各父级轴心已处于画布中心，毕竟属于通用需求
            if (mPos.byWorldOrCanvas)
            {
                mPosCtrl.SuUpdatePosByWorld(mPos.posStart);
            }
            else
            {
                if (mPos.byAbsoluteOrPercent)//与外界事物需要对齐同步
                    ShiftToPos(mPos.byGlobalOrLocal, (Vector2)mPos.posStart + mPos.posOffset);
                else//此时只需面向自己
                    ShiftToPos(false, CoordFrame.SuCoordInCanvasFromScreenByPercent(mPosCtrl.meCanvasLay, mPos.posStart) + mPos.posOffset);
            }
            mIsReady = true;
        }
    }

    public void AcceptToFollow(IfoFollow need)
    {
        mFollow = need;
    }

    //外界可用=================================

    public void StaticInCanvas()//撤除追踪
    {
        mFollow.inNeed = false;
        mFollow.to = null;
    }

    public struct IfoFollow {
        public Transform to;//暂时只在自己需要追踪行径时使用
        public bool inNeed;

        public IfoFollow(Transform to)
        {
            inNeed = true;
            this.to = to;
        }
    }

    public struct Ifo
    {
        public Vector3 posStart;
        //意义会随其它属性的情况而定
        //注意，用的是position属性，而不是anchored position
        public Vector2 posOffset;
        
        public bool byWorldOrCanvas;
        //所接收位置数据是相对画布坐标系描述的，还是场景坐标系
        //如果为true，则位置坐标，直接对应三维场景中的某个地点
        public bool byAbsoluteOrPercent;
        //当相对画布坐标系描述位置时，该变量才有效
        //所接收位置数据为数值坐标，还是百分比坐标
        //如果为false，则位置坐标，是相对二维视野上的百分比位置，且(0,0)点在画布的左下角
        //如果为true，则位置坐标，是相对二维视野上的计量位置，且(0,0)点在画布的中心
        public bool byGlobalOrLocal;
        //当采用标准单位来标定位置时，该变量才有效
        //为true，则认为给的是全局坐标，此时应用到全局坐标属性上
        //为false，则认为给的是局部坐标，此时应用到局部相对坐标属性上
        
        public Ifo(Vector3 posNeed)
        {
            posStart = posNeed;
            posOffset = Vector2.zero;
            byWorldOrCanvas = true;
            byAbsoluteOrPercent = false;
            byGlobalOrLocal = false;
        }
    }
}
