using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollidBody : MonoBehaviour
{//要作为身体物体的子物体上的组件，这样碰撞体才会跟着移动与翻转、伸缩

    [HideInInspector] public BoxCollider2D meStand;//主碰撞体

    BoxCollider2D[] mCollidParts;

    public void MakeReady(HitRespond hitRespond)
    {
        meStand = GetComponent<BoxCollider2D>();
        meStand.isTrigger = true;
        HitRespondPart tmpPart = gameObject.AddComponent<HitRespondPart>();
        tmpPart.MakeReady(hitRespond);

        mCollidParts = new BoxCollider2D[transform.childCount];
        for (int i = 0; i < mCollidParts.Length; i++)
        {
            mCollidParts[i] = transform.GetChild(i).GetComponent<BoxCollider2D>();
            mCollidParts[i].isTrigger = true;
            tmpPart = mCollidParts[i].gameObject.AddComponent<HitRespondPart>();
            tmpPart.MakeReady(hitRespond);
        }
    }

    //============================

}
