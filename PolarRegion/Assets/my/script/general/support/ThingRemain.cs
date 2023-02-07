using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ThingRemain : MonoBehaviour
{//把需要一直保留在场景中的物体，作为挂载了该组件的物体的子物体
 //主要是为了跨场景，也保持存在，同时再跨回来时，也不会出现多个同物体
 //注意任何子物体，如果需要Awake且其过程涉及对共用数据的修改，请继承该组件提供的接口，否则可能出问题，刚跨越的时候，会同时有两个thingRemain

    static ThingRemain it;
    public static ThingRemain It
    {
        get
        {
            if (!it)
                it = GameObject.Find("thing-remain").GetComponent<ThingRemain>();
            return it;
        }
    }

    void Awake()
    {
        //先保证单体的全局性
        if (it != null) Destroy(gameObject);//防止存在多个
        else
        {
            if (It)//这里催促赋上值，避免他人趁机占用了
                DontDestroyOnLoad(gameObject);//唯一时，长期存续
            AwakeOther();
        }
    }

    int mLastSceneIndex;
    ISwitchScene[] mSwitches;

    void AwakeOther()
    {
        mSwitches = GetComponentsInChildren<ISwitchScene>();
        foreach (ISwitchScene sw in mSwitches) sw.WhenAwake();
        //需要awake中执行，因为属于框架类事物
        mLastSceneIndex = SceneManager.GetActiveScene().buildIndex;
    }

    void Update()
    {
        if (mLastSceneIndex != SceneManager.GetActiveScene().buildIndex)
        {//当场景发生切换时
            foreach (ISwitchScene sw in mSwitches) sw.WhenSwitchScene();
            mLastSceneIndex = SceneManager.GetActiveScene().buildIndex;
        }
    }

}

//想要跨场景保存的，都继承该接口
public interface ISwitchScene
{
    void WhenAwake();
    void WhenSwitchScene();
}