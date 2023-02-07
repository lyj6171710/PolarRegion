using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThingRefer : MonoBehaviour
{//ÿ�������У��Ը�������������壬�����临��Ʒ
 //��ÿ���ڲ�ͬ�����У�������ʹ�õ�����������������
 //���ָ��Ƶ�������Ҫ��Դ�ڽṹ��һ����������ϸ���ϲ�һ����������ʽ������������

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
    Dictionary<ERefer, GameObject> mVariants;//�����Ǿ�̬�ģ���Ϊ����ڳ���

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