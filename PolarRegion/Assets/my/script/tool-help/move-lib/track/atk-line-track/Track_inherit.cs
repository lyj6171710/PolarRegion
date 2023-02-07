using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Track_inherit : MonoBehaviour
{
    public GameObject will_create;
    public float speed_change;
    [Space(10)]
    public Vector3 offset_create_from_right = new Vector3(0, 0, 0);
    public float angle_change_from_right;
    //当移向右的弹道自毁时，所另生成弹道的移向，相对该自毁弹道移向的变动
    public Vector3 offset_create_from_left { get { return new Vector3(-offset_create_from_right.x, offset_create_from_right.y, 0); } }//自动计算
    public float angle_change_from_left { get {return - angle_change_from_right ; } }//自动计算
    
    private float temp_speed;
    private float temp_angle;
    private int temp_valid;
    private Transform temp_creator;
    private Vector3 temp_pos_offset;

    private void OnDestroy()
    {
        if (will_create)
        {
            GameObject G = Instantiate(will_create);

            Track_move fade_automove = GetComponent<Track_move>();
            Track_move target_automove = G.GetComponent<Track_move>();
            Track_trait fade_trait = GetComponent<Track_trait>();
            Track_trait target_trait = G.GetComponent<Track_trait>();

            temp_speed = fade_automove.speed + speed_change;//速度继承

            temp_valid = fade_automove.valid_face;//有效方向继承

            if (fade_automove.direction.x >= 0)//当弹道移向右时
                temp_angle = fade_automove.angle + angle_change_from_right;//移向继承
            else
                temp_angle = fade_automove.angle + angle_change_from_left;
            
            {//跟随状态继承
                temp_creator = fade_automove.creator;
                if (fade_automove.direction.x >= 0)
                    temp_pos_offset = fade_automove.pos_offset + fade_automove.offset + offset_create_from_right;//自身偏移加新增偏移
                else
                    temp_pos_offset = fade_automove.pos_offset + fade_automove.offset + offset_create_from_left;
            }//注意只是继承了想要继续跟随时的前提，弹道相对发射者的关系并没有继承，需要另有主动设定

            target_automove.assign_move_(temp_creator, temp_angle, temp_speed, temp_pos_offset, temp_valid);//初始化配置

            //target_trait.apply_trait_(fade_trait.issuer_type);//属性继承
        }
    }
}
