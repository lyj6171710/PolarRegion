using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Nature { physics, magic, spirit }//数值特性
public enum Convertion { none, 血量 , 蓝量,  物攻, 法攻}//转化对象
public enum Ctrl_hard { none, 眩晕, 击飞, 击退, 吸近 }//行为性硬控效果
public enum Ctrl_soft { none, 毒素, 渐移, 沉默 }//行为性软控效果
public enum Alter_attr { none, 移速, 物攻, 物防, 法攻, 法防, 魂力}//属性影响效果

public class Track_harm {
    public Track_trait trait;
    public float harm;

    public Track_harm(Track_trait trait, float harm)
    {
        this.trait = trait;
        this.harm = harm;
    }
}

public class Track_trait : MonoBehaviour
{
    public Nature nature_self = Nature.physics;//伤害类型
    public float attack_self = 0;//自身基础伤害力
    public float add_scale = 1;//伤害加持比例
    public Nature nature_to_add = Nature.physics;//加持对象
    
    [Space(25)]
    public Ctrl_hard ctrl_hard = Ctrl_hard.none;//硬控类型
    public float duration_hard = 0;//这里的时间，本质意义是力度，力度越大，应被硬控越久

    [Space(25)]
    public Ctrl_soft ctrl_soft = Ctrl_soft.none;//软控类型
    public float duration_soft = 0;

    [Space(25)]
    public Alter_attr alter_attr = Alter_attr.none;//属性影响类型
    public float duration_attr = 0;
    public float difference = 0;
    
    [Space(25)]
    public Convertion convert=Convertion.none;//将转化到哪一类型下的值
    public float convert_ratio = 0;//百分比
    public float duration_convert = 0;
    
    [Header("下方各变量值，不需手动变换")]
    //public Type_of issuer_type;//这里的Type_of组件视为人物属性对外的统一接口
    //public Type_of type;
    //public Camp issuer_camp;//弹道所属阵营
    public float issuer_attack;//广义上的攻击力
    public float disposable_time;//受该弹道只一次作用的时段上限，一般与弹道从生成起到其多久自毁的时隔一致
    private float timer;//计时器
    public float Time_remanent { get { return disposable_time - timer; }  }//自毁倒计时

    private void Start()
    {
        disposable_time = GetComponent<Track_move>().wait_time;//这里强制等同于将自毁的时间，且该值是预设好的
    }

    private void FixedUpdate()
    {
        if (Time_remanent > 0)
            timer += 0.02f;
        //type.set_attrf_("remanent", Time_remanent);
    }
    
    public void feedback_(float harm)
    {
        Track_harm track=new Track_harm(this,harm);
        //issuer_type.gameObject.SendMessage("receive_feedback_",track);
    }
    
    //public void apply_trait_(Type_of source)
    //{
    //    if (nature_to_add == Nature.physics)
    //    {
    //        if (!source.get_attrf_("物攻", ref issuer_attack))
    //            issuer_attack = 0;
    //    }
    //    else if (nature_to_add == Nature.magic)
    //    {
    //        if (!source.get_attrf_("法攻", ref issuer_attack))
    //            issuer_attack = 0;
    //    }
    //    else
    //    {
    //        if (!source.get_attrf_("魂力", ref issuer_attack))
    //            issuer_attack = 0;
    //    }
    //    issuer_type = source;
    //    issuer_camp = source.camp;
    //    type = GetComponent<Type_of>();
    //    type.camp = issuer_camp;
    //}
    
}
