using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeqPlayer : MonoBehaviour {//只负责播放或后续处理，受管理者临时生成
    
    //基本参数----------------------------------------
    public AnimationClip seq;

    public SeqRule use;
    
    //计算结果-------------------------------------
    float period;
    float frameRate;
    float totalFrame;
    
    //内部使用-------------------------------------
    Animator animator;
    SpriteRenderer render;
    public RuntimeAnimatorController medium;
    string loader_name;
    float cur_frame;//加速作用

    bool have_ready = false;
    
    //内部机制=============================

    void Update()
    {
        if (!have_ready) return;

        cur_frame = get_cur_frame_();

        if (!use.reverse)
        {
            if (totalFrame - cur_frame < 2)//还剩一帧时，结束动画
            {
                if (use.auto_die) end_play_();
                else pause_();
            }
        }
        else
        {
            if (cur_frame >= 1 && cur_frame <= 3) //不能是小于或大于2，当前逻辑流程所致的倒放，会先是0，然后再是从最大帧数下降到0
            {
                if (use.auto_die) end_play_();
                else pause_();
            }
        }
    }
    
    void attr_ready_()
    {
        render = gameObject.AddComponent<SpriteRenderer>();
        animator = gameObject.AddComponent<Animator>();
        render.sortingOrder = 2;//应有统一规定，0地面，1对象，2前景

        //AnimationClip[] clips = medium.animationClips;//先引用出来，得知信息
        //loader_name = clips[0].name;//得知并存储索引信息
        loader_name = "default";//可以直接预定，控制器是专门设置的，没必要遍历，由此加速

        AnimatorOverrideController override_medium = new AnimatorOverrideController();
        override_medium.runtimeAnimatorController = medium;
        seq.wrapMode = WrapMode.Once;

        override_medium[loader_name] = seq;//未知原因，需要这样赋值才有效，而不是用overridemedium.animationClips
                                           //用动画资源的名称与动画控制器中特定的动画块对应
        animator.runtimeAnimatorController = null;//听说不加这句，会造成错乱或内存耗费不断增加
        animator.runtimeAnimatorController = override_medium;
    }
    
    void analyse_seq_()
    {
        //动画片段长度
        period = animator.GetCurrentAnimatorClipInfo(0)[0].clip.length;
        //获取动画片段的每秒帧数
        frameRate = animator.GetCurrentAnimatorClipInfo(0)[0].clip.frameRate;
        //计算动画片段总帧数
        totalFrame = period / (1 / frameRate);
    }
    
    void end_play_()
    {
        animator.enabled = false;
        render.enabled = false;
        Destroy(gameObject);
    }

    float update_speed_()
    {
        float speed = Mathf.Clamp(use.speed, 0.01f, 5);
        if (use.reverse)
        {
            animator.StartPlayback();//animator.speed才能赋负值
            animator.speed = -speed;
        }
        else
            animator.speed = speed;
        return speed;
    }

    //内外机制===============================

    public void build_(AnimationClip need, SeqRule rule, RuntimeAnimatorController media)//其它接口能够使用前的前提
    {
        seq = need;
        medium = media;//这个media是特定的，不是随便一个就行

        use = new SeqRule();
        use.copy_from_(rule);

        attr_ready_();
        
        analyse_seq_();//识别分析，方便后续处理

        update_speed_();

        have_ready = true;
    }
    
    //外界可用==============================

    public void play_anew_()//默认播放一轮就结束
    {//从头开播
        if (!use.reverse)
        {
            update_speed_();
            animator.Play(loader_name, 0, 0);
        }
        else
        {
            animator.StartPlayback();
            update_speed_();
            animator.Play(loader_name, 0, 1);
        }
    }

    public void pause_()
    {
         animator.speed = 0;
    }

    public float set_speed_(float speed)
    {
        use.speed = speed;
        update_speed_();
        return speed;
    }

    //内部工具==============================
    
    int get_cur_frame_()
    {
        //当前动画机播放时长
        float currentTime = animator.GetCurrentAnimatorStateInfo(0).normalizedTime;//信息需要在当前帧重新获取
        //计算当前播放的动画片段运行至哪一帧
        return (int)(Mathf.Floor(totalFrame * currentTime) % totalFrame);
        //Debug.Log(" Frame: " + currentFrame + "/" +totalFrame);
    }

    float get_progress_()//百分比会有跳跃，不建议使用
    {
        AnimatorStateInfo Info = animator.GetCurrentAnimatorStateInfo(0);
        return Info.normalizedTime;
    }

}

[System.Serializable]
public class SeqRule
{
    public float speed = 1;
    public bool reverse = false;
    public bool auto_die = true; //动画到最后一帧后，是否只是停止

    public void copy_from_(SeqRule other)
    {
        speed = other.speed;
        reverse = other.reverse;
        auto_die = other.auto_die;
    }
}