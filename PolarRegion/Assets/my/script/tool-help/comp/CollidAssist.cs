using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollidAssist
{
    public static Vector3 GetTriggerHitPos(Collider2D other,Transform self)
    {//如果对方是触发器
        return other.bounds.ClosestPoint(self.position);//通过获取离该包围盒上最近的点来大致判断
    }

    public static Vector3 GetColliderHitPos(Collision2D other)
    {//如果对方是碰撞器
        return other.contacts[0].point;
    }
}
