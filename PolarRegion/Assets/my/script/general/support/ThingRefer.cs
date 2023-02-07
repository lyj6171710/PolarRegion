using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThingRefer : MonoBehaviour
{//每个场景中，对该组件所挂载物体，都有其复制品
 //让每个在不同场景中，被复制使用的事物，都被该组件引用
 //这种复制的需求，主要来源于结构上一样，但参数细节上不一样，共用形式并不利于运用

    static ThingRefer it;
    public static ThingRefer It
    {
        get
        {
            if (!it)
                it = GameObject.Find("thing-refer").GetComponent<ThingRefer>();
            return it;
        }
    }

    public Info[] mVariantsDp;
    Dictionary<ERefer, GameObject> mVariants;//不能是静态的，因为相关于场景

    public GameObject this[ERefer match] { get {
            if (mVariants == null)
            {
                mVariants = new Dictionary<ERefer, GameObject>();
                foreach (Info one in mVariantsDp)
                {
                    if (!mVariants.ContainsKey(one.match))
                        mVariants.Add(one.match, one.which);
                }
            }
            return mVariants[match];
        }
    }

    [System.Serializable]
    public struct Info
    {
        public ERefer match;
        public GameObject which;
    }
}

public enum ERefer { OverlayNormalCanvas }