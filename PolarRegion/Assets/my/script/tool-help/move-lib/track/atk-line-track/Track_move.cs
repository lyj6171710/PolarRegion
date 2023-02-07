using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Track_move : MonoBehaviour
{
    public float angle { get; set; }//移动方向
    public float speed { get; set; }//移动速度
    public Vector2 direction { get; set; }//移动方向的等效坐标表示
    public Transform creator { get; set; }//弹道所相对的发弹者
    Vector3 forward { get; set; }//具有移动方向及速度信息的坐标表示
    Vector3 offset_per_frame { get; set; }//每帧需有的位置偏移
    public Vector3 offset { get; set; }//多帧累积起的偏移量
    public Vector3 pos_offset { get; set; }//弹道初始位置相对发弹者位置所具有的偏移
    public int valid_face { get; set; }//有效朝向，该实例所对应的发弹者朝向
    public float last_face { get; set; }//记录发弹者上一帧时的朝向

    public bool whether_follow;//弹道是否跟随发弹者的移动而移动
    public bool whether_same_side;//弹道是否跟随发弹者朝向的转变而跳转到对称位置
    public bool face_to_source;//弹道图像自身某侧边是否总是朝向发弹者

    [Space(10)]
    public bool 一定时间后自毁 = false;
    public float wait_time;

    public bool 遇到碰撞体自毁 = false;
    public List<string> meet;

    //算法支撑
    float temp;//临时使用

    Rigidbody2D rigid;//自身刚体
    CapsuleCollider2D collide;//自身碰撞体
    SpriteRenderer sprite;//自身图像
    Track_trait trait;//自身当前属性

    private void OnTriggerEnter(Collider other)
    {
        //if (遇到碰撞体自毁)
        //    on_trigger_enter_(other.GetComponent<Type_of>());
    }

    //public void on_trigger_enter_(Type_of trigger)
    //{
    //    for (int i = 0; i < meet.Count; i++)
    //    {
    //        if (trigger.name == meet[i])
    //            Destroy(gameObject);
    //    }
    //}

    private void Start()
    {
        getcomponent_();

        start_move_();

        take_effect_();
        
        //可见性、谁可见
        whether_visible_(false);//先全部关闭，只有通过检测者才会显化
        if (creator)
        {
                if (valid_face > 0 && creator.localScale.x > 0)//要求素材默认就是朝右的
                    whether_visible_(true);
                else if (valid_face < 0 && creator.localScale.x < 0)
                    whether_visible_(true);
                else whether_visible_(false);
        }
        else
        {//此时按双向方式发射两个弹道，或弹道在竖直方向上移动，都可以为显化状态
            whether_visible_(true); 
        }
    }

    private void FixedUpdate()
    {
        if(creator)//先需有发弹者
        {
            if (whether_follow)//是否跟随
            {
                offset += offset_per_frame;
                transform.position = creator.position + pos_offset + offset;//位置受发弹者影响

                //////////////////////////////////////////////////////////////////////////////
                ///
                if (whether_same_side)//是否同侧(要先需跟随，同侧才有意义)
                {
                    if (valid_face > 0 && creator.localScale.x > 0)
                        whether_visible_(true);
                    else if (valid_face < 0 && creator.localScale.x < 0)
                        whether_visible_(true);
                    else whether_visible_(false);

                    /////////////////////////////////////////////////////////////////////////////////////
                    ///
                    if (face_to_source)//是否朝向发弹者(要先需同侧，总相对的朝向才有意义)
                    {
                        //if ((temp = trait.issuer_type.transform.localScale.x) != last_face)
                        //{//注意两对称弹道在图片上并没有设置成对称，因此需要主动判断
                        //    transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, 0);
                        //    last_face = temp;
                        //}
                    }
                }
            }
        }
    }

    void start_move_()
    {
        float radian = Mathf.PI * angle / 180;
        direction = new Vector2(Mathf.Cos(radian), Mathf.Sin(radian));
        forward = direction * speed;
        
        if (whether_follow && creator)
            offset_per_frame = forward / 50;
        else
            rigid.velocity = forward;//不用跟随时，只需指定一个绝对速度，自行移动即可
    }
    
    public void getcomponent_()
    {//弹道效果所涉及的组件
        rigid = GetComponent<Rigidbody2D>();
        collide = GetComponent<CapsuleCollider2D>();
        sprite = GetComponent<SpriteRenderer>();
        trait = GetComponent<Track_trait>();
    }

    public void take_effect_()
    {
        if (一定时间后自毁)
            Destroy(gameObject, wait_time);
    }

    public void whether_visible_(bool visible)
    {
        if (visible)
        {
            collide.enabled = true;
            sprite.enabled = true;
        }
        else
        {
            collide.enabled = false;
            sprite.enabled = false;
        }
    }
    
    public void assign_move_(Transform creator, float angle_will,float speed_will, Vector3 pos_offset_will,int accordance)
    {
        angle = angle_will;//移动角度
        speed = speed_will;//移动速度
        pos_offset = pos_offset_will;//初始位置相对发弹者的偏移
        this.creator = creator;//发弹者
        valid_face = accordance;//当前实例对应的发弹者朝向
        last_face = creator.transform.localScale.x;//发弹者当前朝向

        //初始位置
        transform.position = creator.position + pos_offset_will;

        //图像自身的初始旋转状态
        if (creator.transform.localScale.x < 0)//默认要求弹道图像自身的方向就是其朝向当发弹者朝右边发射时的方向
            transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, 0);//要0度，要么180度
    }

}

    
