using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Track_create : MonoBehaviour
{
    /*理念：
     * 1.弹道始终以它的发弹者作为基本参照，不如此者都是得特殊处理的，不需、不应用代码兼容
     * 2.这里的弹道指，造成攻击轨迹的潜在实体，弹道实体是预置有的，所有角色选择弹道以某种方式使用而已（也就是弹道的属性与使用分开看待）
     */

    //弹道系统：Track_create、Track_inherit、Track_move、Track_trait
    //前三者管理弹道的移动，最后一项管理弹道自身属性
    //每次弹道的发射，都有一个对称弹道，可用来及时更换位置

    private static Track_create single;
    public static Track_create Single
    {
        get
        {
            if(!single)
            single = GameObject.Find("track_creator").GetComponent<Track_create>();
            return single;
        }
    }
    
    Vector3 pos_offset_left;//生成点相对发射者中心的偏移
    Vector3 pos_offset_right;
    float angle_from_left;
    float angle_from_right;

    public void create_track_(GameObject create,Transform creator,float angle,float speed,Vector3 relative_offset)
    {//当发弹者以朝右边的自身状态发射出弹道时，弹道初始的位置及发射方向、发射速度是多少

        //对称化数据
        angle_from_right = angle;
        angle_from_left = 180 - angle;
        pos_offset_right = relative_offset;
        pos_offset_left = new Vector3(-relative_offset.x, relative_offset.y, 0);
        
        //左边弹道的初始化设定(绝对性的左边)
        GameObject track_left = Instantiate(create);
        track_left.GetComponent<Track_move>().assign_move_(creator, angle_from_left, speed, pos_offset_left, -1);//初始化移动
        //默认+1对应右边，-1对应左边
        //track_left.GetComponent<Track_trait>().apply_trait_(creator.GetComponent<Type_of>());//初始化属性
        

        //右边弹道的初始化设定(绝对性的右边)
        GameObject track_right = Instantiate(create);
        track_right.GetComponent<Track_move>().assign_move_(creator, angle_from_right, speed, pos_offset_right, 1);
        //track_right.GetComponent<Track_trait>().apply_trait_(creator.GetComponent<Type_of>());
    }
}
