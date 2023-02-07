using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowInScene : MonoBehaviour {
    
    //基本参数-------------------------------------

    public Transform follow_to;
    public bool whether_follow;
    public Vector3 follow_offset;//暂时只考虑固定偏移，理想情况应该是
                                 //外界控制该偏移，可时刻调整动画当前应处于的位置

    //内部使用------------------------------------

    bool have_set = false;

    //内部机制=================================
    
	void Update () {
        if (!have_set) return;
        if (whether_follow && follow_to)//如果需要跟随，则时刻刷新位置
            transform.position = follow_to.position + follow_offset;
    }

    //内外机制==================================

    public void set_accord_(FollowRule rule)
    {
        follow_to = rule.follow;
        follow_offset = rule.offset;
        whether_follow = rule.whether_follow;

        transform.position = rule.pos;

        Vector3 tmp_pos = transform.localScale;
        transform.localScale = new Vector3(tmp_pos.x * rule.size, tmp_pos.y * rule.size, tmp_pos.z);
        //有未知bug，如果存在有其它序列帧控制了缩放，这里就会无效，解决方式是讲控制了缩放的动画从控制机中移除

        have_set = true;
    }

}

[System.Serializable]
public class FollowRule
{
    public Vector3 pos=default(Vector3);
    public float size = 1;
    public bool whether_follow = false;
    public Transform follow = null;
    public Vector3 offset = default(Vector3);
}