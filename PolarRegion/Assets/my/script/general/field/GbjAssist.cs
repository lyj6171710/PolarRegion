using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GbjAssist : MonoBehaviour
{
    public static Transform GetSuper(Transform which)//获得最上级
    {
        Transform super = which;
        while (super.parent != null)
        {
            super = super.parent;
        }
        return super;
    }

    public static Vector3 GetSumScaleWhenParent(Transform self)
    {
        //Transform super = self;
        //Vector2 localBasisAcc = Vector2.one;
        //while (super.parent != null)
        //{
        //    super = super.parent;
        //    localBasisAcc = GlbAssist.Product(localBasisAcc, super.localScale.ToVector2());
        //}
        //return localBasisAcc;

        if (self.parent != null) 
            return self.parent.lossyScale;
        else 
            return Vector3.one;
    }

    public static Vector3 GetSumScaleWhenSelf(Transform self)
    {
        Vector3 scaleAcc = GetSumScaleWhenParent(self);
        return MathVector.Product(scaleAcc, self.localScale);
    }

    public static Vector3 GetSumRotWhenParent(Transform self)
    {
        if (self.parent != null) return self.parent.eulerAngles;
        else return Vector3.zero;
    }

    public static Vector3 GetLocalPosInScene(Transform self)//相对场景的相对距离，不受缩放影响
    {
        return MathVector.Product(self.localPosition, GetSumScaleWhenParent(self));
        //物体自身的缩放只影响其子物体的偏移距离单位
    }


    //==========================

    // Start is called before the first frame update
    public static T AddCompSafe<T>(GameObject mount) where T : Component
    {
        T find = mount.GetComponent<T>();
        if (find != null)
        {
            return find;
        }
        else
            return mount.AddComponent<T>();
    }

    public static void ClearChilds(Transform parent)
    {
        //for (int i = 0; i < parent.childCount;i++)
        //{
        //    Destroy(parent.GetChild(i).gameObject);
        //    //Destroy 并不会立刻改变 childCount，
        //    //DestroyImmediate 会立刻改变 childCount
        //}

        for (int i = 0; i < parent.childCount;)
        {
            DestroyImmediate(parent.GetChild(i).gameObject);
        }
    }

    public static GameObject AddChildNormal(Transform parent, string name = "child")
    {
        GameObject child = new GameObject(name);
        child.transform.SetParent(parent);
        child.transform.localPosition = Vector3.zero;
        return child;
    }

    public static T GetCompInUpper<T>(Transform self) where T : Component
    {//包括在self中寻找
        Transform belong = self;
        while (belong != null)
        {
            T find = belong.GetComponent<T>();
            if (find != null)
                return find;
            else
                belong = belong.parent;
        }
        return null;
    }

    public static GameObject FindSubGbjByName(Transform parent,string name)
    {
        if (name == null || name == "") return null;
        Transform find = parent.Find(name);
        if (find == parent)
            return null;
        else
            return find.gameObject;
    }

    public static GameObject[] GetSubGbjs(Transform belong)
    {
        GameObject[] list = new GameObject[belong.childCount];
        for (int i = 0; i < belong.childCount; i++)
        {
            list[i] = belong.GetChild(i).gameObject;
        }
        return list;
    }

    //============================

    public static void ResetTransform(Transform self, bool includePos = true)
    {
        if (includePos) self.transform.position = Vector3.zero;
        self.rotation = Quaternion.identity;
        self.localScale = Vector3.one;
    }

}
