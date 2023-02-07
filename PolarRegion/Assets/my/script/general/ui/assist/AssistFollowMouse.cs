using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AssistFollowMouse :AssistFollow//需提前附着在预制体上，然后由外界酌情启用黏附效果
{//追随鼠标位置的效果
 //还未修改完善
    protected override void UpdateNer()
    {
        if (mFollow)
        {
            if (mMode == Mode.point)
            {
                mNeed = UnifiedCursor.It.SuGetCursorLocate(mPos.meCanvasLay) / mPos.meCanvasLay.localScale;
            }
        }
    }

    protected override void StartNer()//拖拽前需要的准备操作
    {
        //这里预定被拖拽的物体，应该只是一个临时物体
        transform.SetParent(mPos.meCanvasLay.transform, false);//降低牵连
    }

    
}
