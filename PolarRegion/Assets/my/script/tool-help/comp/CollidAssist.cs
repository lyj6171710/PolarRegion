using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollidAssist
{
    public static Vector3 GetTriggerHitPos(Collider2D other,Transform self)
    {//����Է��Ǵ�����
        return other.bounds.ClosestPoint(self.position);//ͨ����ȡ��ð�Χ��������ĵ��������ж�
    }

    public static Vector3 GetColliderHitPos(Collision2D other)
    {//����Է�����ײ��
        return other.contacts[0].point;
    }
}
