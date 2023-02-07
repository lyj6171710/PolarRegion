using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AssistOption : MonoBehaviour,AssistClick.ITouch
{
    //被AssistSelect使用
    //对应一个可选选项整体

    AssistSelect mLeader;
    AssistClick mMouse;
    AssistPos mPos;
    public AssistPos meSitu => mPos;

    int mIndex;
    
    public void MakeReady()
    {
        mPos = GbjAssist.AddCompSafe<AssistPos>(gameObject);
        mPos.MakeReady();
    }

    public void MakeUse(AssistSelect leader, int index)
    {
        mLeader = leader;
        mIndex = index;

        mMouse = gameObject.AddComponent<AssistClick>();
        mMouse.SuReactTouch(this);
    }//该组件应可以被复用

    public void MakeDown()//不再需要选择时，清空状态，等待再次被使用
    {
        mLeader = null;
        mIndex = -1;
        Destroy(mMouse);
        //Destroy(mPos);//位置辅助没有必要移除，其低依赖于外界情况，何况还可能再用到
    }

    //处理表现效果，不含机制影响========================

    public virtual void FollowEffectHover(bool onoff) { }
    public virtual void FollowNotValid(bool hide) { }
    public virtual void FollowEffectClick() { }

    //==================================================

    protected void ExciteLeaderHover(int part) 
    {
        mLeader.ExciteSelectByOption(mIndex, part);
    }

    protected void ExciteLeaderClick(int part)
    {
        mLeader.ExciteConfirmByOption(mIndex, part);
    }

    public void iHoverInsideFmMouse(object para)
    {
        ExciteLeaderHover(0);
    }

    public void iHoverOutsideFmMouse(object para)
    {
        
    }

    //===========================================


}
