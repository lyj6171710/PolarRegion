using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AssistFollow : MonoBehaviour////需提前附着在预制体上，然后由外界酌情启用黏附效果
{//这是基类，不能直接使用，需要使用继承该组件的组件
 //挂载该组件的物体，还得挂载有AssistPos的组件

    //外界可操纵=================================

    public bool mFollow;
    public Mode mMode = Mode.point;

    public virtual void BackToStart(){ }

    //私用变量==================================

    bool mHaveReady = false;
    protected AssistPos mPos;//需提供一系列位置数据的帮助
    protected RectTransform meSelf { get { return mPos.meSelfLay; } }
    protected Vector2 mNeed;//子类去操作这个变量，意义随跟随模式的不同而不同
    protected bool mForce;//子类有机会进行一次强制位置调整

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
            mForce = false;//自动关闭
        }
    }

    void UpdatePos()//根据当前数据，更新表现
    {
        if (mMode == Mode.point)
        {
            //此时mNeed的数值单位是UI画布的长度单位
            meSelf.localPosition = mNeed;
        }
        else
        {
            meSelf.localPosition += new Vector3(mNeed.x, mNeed.y, 0);
            mNeed = Vector2.zero;
        }
    }


    //架构需要========================================

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

    public enum Mode { point,offset }//跟随模式
}
