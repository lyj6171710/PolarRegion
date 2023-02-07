using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShowHp : MonoBehaviour{
    //需要挂载到用来显示血量的物体集中的最高层物体上
    //提前装入预制体，且需要图片元件自己先设定好属性，该组件才能成功控制

    //外界可操作==================================

    //一次性赋值==================================

    public Image mBufferDp;
    public Image mBloodDp;
    public Text mValueDp;

    //私用变量======================================

    Image mBack;//血条的背景，一般就是黑色的，与当前血量发生对比

    bool mVaryJust;//刚刚受伤
    float mMaxHp;//显示文本值
    float mNowHp;
    float mRatioNow;

    //内部机制=========================================

    private void Awake()
    {
        mBack = GetComponent<Image>();
    }

    void Update()
    {
        if (mVaryJust)
        {
            mBufferDp.fillAmount = Mathf.Lerp(mBufferDp.fillAmount, mBloodDp.fillAmount, 0.03f);
            if (mBufferDp.fillAmount == mBloodDp.fillAmount) mVaryJust = false;
        } 
    }
    
    IEnumerator ProgressiveShow()//整个血条渐渐出现
    {
        float cur = 0;
        SetOpacity(cur);
        while (cur < 1)
        {
            cur += 0.05f;
            SetOpacity(cur);
            yield return new WaitForSeconds(0.1f);
        }
        SetOpacity(1);
    }

    void SetOpacity(float aim)//设置整体的不透明度
    {
        aim = Mathf.Clamp(aim, 0, 1);
        mBack.color = new Color(mBack.color.r, mBack.color.g, mBack.color.b, aim);
        mBloodDp.color = new Color(mBloodDp.color.r, mBloodDp.color.g, mBloodDp.color.b, aim);
        mBufferDp.color = new Color(mBufferDp.color.r, mBufferDp.color.g, mBufferDp.color.b, aim);
    }

    void BufferEndInstant()//缓冲瞬间结束
    {
        mBufferDp.fillAmount = mBloodDp.fillAmount;
    }
    
    void SetRatio(float ratio)//改变血条状态的根本方式
    {
        ratio = Mathf.Clamp(ratio, 0, 1);
        if (ratio != mRatioNow)
        {
            mRatioNow = ratio;
            mBloodDp.fillAmount = mRatioNow;
            mVaryJust = true;
        }
    }

    void SetHp(float hp_now,float hp_max)
    {
        mNowHp = hp_now;
        mMaxHp = hp_max;
        if (mValueDp) mValueDp.text = hp_now.ToString("0.0") + " / " + hp_max.ToString("0.0");//有时不一定需要显示出具体值
        SetRatio(mNowHp / mMaxHp);
    }

    //外界可用================================

    public void SuSetSize(float scale)
    {
        GetComponent<RectTransform>().localScale = new Vector3(scale,scale,1);
    }

    public void SuOpen()
    {
        StartCoroutine(ProgressiveShow());
    }

    public void SuClose()
    {
        BufferEndInstant();
    }

    public void SuRefresh(float cur,float max)
    {
        SetHp(cur, max);
    }
}