using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GObject
{
    public static void BuildAndHold(GameObject hold, Transform holder, out SpriteRenderer render, out BoxCollider2D collid)
    {//内容是相对的，除了图像和碰撞
        hold.transform.SetParent(holder);
        hold.transform.localPosition = Vector3.zero;
        hold.transform.localRotation = Quaternion.identity;

        render = hold.AddComponent<SpriteRenderer>();
        render.enabled = false;

        collid = hold.AddComponent<BoxCollider2D>();
        collid.isTrigger = true;
    }

}
