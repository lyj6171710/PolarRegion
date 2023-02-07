using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class UiGenerate : MonoBehaviour, ISwitchScene
{
    //用来帮助生成与运用，那些只临时用的UI元素

    //不同场景，最好是复制关系，而不是共用同一个，因为不同场景下的画布比例可能会不同，这会导致参数失效

    //一次性赋值=======================

    public List<GameObject> mStyleReferDp;//只要装载有特定组件即可的预制体，承担预置风格的功能

    public void WhenAwake()
    {
        transform.GetComponentInParent<Canvas>().sortingOrder = GlobalConfig.prior_ui;//让所处画布优先显示
        AwakeOther();
    }

    public void WhenSwitchScene()
    {
    }

    protected virtual void AwakeOther()
    {

    }

    //根本===============================================

    protected UiAlone FormAlone(GameObject prefabRefer, UiAlone.Ifo refer, float size = 1)
    {
        UiAlone alone = FormAlone(prefabRefer, size);
        alone.AcceptToLocate(refer);
        alone.MakeReady();
        return alone;
    }

    //扩展=============================================

    protected UiAlone FormAlone(int style, UiAlone.Ifo refer, float size = 1)
    {
        return FormAlone(mStyleReferDp[style], refer, size);
    }

    //内部工具===========================================

    UiAlone FormAlone(GameObject refer, float size = 1)
    {
        GameObject tmp = Instantiate(refer);
        tmp.transform.SetParent(transform, false);//如果没有false，会默认实例化在世界坐标
        tmp.GetComponent<RectTransform>().sizeDelta *= size;
        //具体事物，最好还得让内容去适配这个大小，否则可能会无法显示
        //比如text，就得GetComponentInChildren<Text>().resizeTextForBestFit = true;
        UiAlone alone = tmp.AddComponent<UiAlone>();
        return alone;
    }
}
