using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ProSideForm
{//在身边圆周线上生成指定物体

    public static GameObject PopCircle(GameObject prefab,Transform area,float radius,float angle)
    {
        Vector3 pos = area.position + (MathAngle.AngleToVector(angle) * radius).ToVector3();
        GameObject item = Object.Instantiate(prefab, area);
        item.transform.position = pos;
        return item;
    }

    public static GameObject PopCircle(Transform belong, float radius, float angle)
    {
        Vector3 pos = belong.position + (MathAngle.AngleToVector(angle) * radius).ToVector3();
        GameObject item = new GameObject();
        item.transform.SetParent(belong.transform);
        item.transform.position = pos;
        return item;
    }

}
