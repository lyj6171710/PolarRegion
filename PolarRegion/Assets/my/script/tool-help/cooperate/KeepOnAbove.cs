using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeepOnAbove : MonoBehaviour
{//让一个子物体始终在其父物体的上方(下方时为负)，这个方向不受本地空间影响
 //该组件挂载于子物体上

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
    float mHeightAt;//相对场景以及父物体描述距离

    float mHeightOffset;//不受上下限影响

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
        //不能是transform.position，旋转时会变化其世界坐标中的x
        float posY = transform.parent.position.y + mHeightOffset + mHeightAt;
        transform.position = new Vector3(origin.x, posY, origin.z);
        //父物体位置可能时刻都在变化
    }

}
