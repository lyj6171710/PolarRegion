using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeqMaker : MonoBehaviour,ISwitchScene
{
    
    public RuntimeAnimatorController spec_controller;//通用的控制器，保持一个特定初始状态即可
    //默认要求所给状态机中，只含一个动画块被连接，该动画块由入口直接连入，同时不连接到出口
    //要该动画块的animationClips属性有赋值，任一会占用一定时间的动画片段，没有内容也行
    //所承载动画片段、动画资源的名称要为default
    //用该组件所挂载物体的动画控制机也是可以的，顺便能方便动画添加与测试

    public List<Deposit> deposit;//动画寄存，外界可以直接通过名称引用使用，或者取出它
    
    void Start()
    {
        GetComponent<SpriteRenderer>().enabled = false;//关闭自用组件
        GetComponent<Animator>().enabled = false;//这两个组件仅仅是用来测试的，游戏时关掉
    }

    //外界可用====================================

    public AnimationClip take_out_seq_(string call)//外界了取出寄存在这里的动画
    {
        AnimationClip target;
        for (int i = 0; i < deposit.Count; i++)
        {
            if (deposit[i].call == call)
            {
                target= deposit[i].anim;
                deposit.RemoveAt(i);
                return target;
            }
        }
        return null;
    }

    public float play_seq_(string call, SeqCtrl ctrl = null)//播放相关动画，随后销毁
    {
        float size;
        AnimationClip need = get_anim_(call, out size);
        if (need) return play_seq_(need, ctrl);
        else Debug.Log("不存在指定序列");
        return 0;
    }

    public float play_seq_(AnimationClip need, SeqCtrl ctrl = null)//播放一遍指定动画，随后销毁
    {
        Transform tmp = add_seq_(need, ctrl).transform;
        SeqPlayer seq = tmp.GetComponent<SeqPlayer>();
        if (ctrl == null)
            return need.length;
        else
            return need.length / seq.set_speed_(ctrl.seq_rule.speed);//外界可以用来适时触发事件
    }

    FollowInScene add_seq_(AnimationClip need, SeqCtrl ctrl = null)
    {//后续位置与大小交给外界控制
        if (need == null || spec_controller == null)
        {
            Debug.Log("参数错误");
            return null;
        }

        SeqPlayer tmp_player = add_a_player();

        if (ctrl == null) ctrl = new SeqCtrl();
        Vector3 pos_normal = new Vector3(ctrl.follow_rule.pos.x, ctrl.follow_rule.pos.y, 0);//外界要注意，不要将z轴值重设时，超过摄像机z轴值
        ctrl.follow_rule.pos = pos_normal;
        FollowInScene tmp_follow = tmp_player.GetComponent<FollowInScene>();
        tmp_follow.set_accord_(ctrl.follow_rule);

        tmp_player.build_(need, ctrl.seq_rule, spec_controller);
        tmp_player.play_anew_();

        return tmp_follow;
    }

    //内部工具=======================================

    SeqPlayer add_a_player()
    {
        GameObject player = new GameObject("seq_player");
        player.AddComponent<FollowInScene>();
        return player.AddComponent<SeqPlayer>();
    }

    AnimationClip get_anim_(string call,out float scale)
    {
        for (int i = 0; i < deposit.Count; i++)
        {
            if (deposit[i].call == call)
            {
                scale = deposit[i].scale;
                return deposit[i].anim;
            }
                
        }
        scale = 1;
        return null;
    }

    //架构===========================================
    
    [System.Serializable]
    public struct Deposit
    {
        public string call;
        public AnimationClip anim;
        public float scale;
    }

    public static SeqMaker It;
    
    public void WhenAwake()
    {
        It = this;
    }

    public void WhenSwitchScene()
    {

    }
}

[System.Serializable]
public class SeqCtrl
{
    public SeqRule seq_rule;
    public FollowRule follow_rule;

    public SeqCtrl() { seq_rule = new SeqRule();follow_rule = new FollowRule(); }
}
