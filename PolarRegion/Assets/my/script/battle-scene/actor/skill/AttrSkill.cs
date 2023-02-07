using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttrSkill : MonoBehaviour
{
    public Ifo mMechanismDp;
    
    [System.Serializable]
    public class Ifo//不影响技能效果的那些属性
    {
        public float hpCost;//消耗多少血量
        public float tpCost;//消耗多少气力
        public float cooling;//冷却
        public float range;//施法范围
        public bool moveLocal;//脱离角色移动，还是相对角色移动
        public EPR_SkillTrack track;
        [MultEnum] public EPR_SkillEffectNature nature;
        public ECamp campTo;

        //================================
        
    }

    [System.Serializable]
    public class IfoAttach//高级属性(暂时不考虑)
    {
        public float preTime;//从启动到能造成效果的时间差，前摇
        public float postTime;//从能造成效果到角色回到待出击状态的时间差，后摇
        //前摇、后摇期间，一般情况下，都无法做出其它行动
        public bool canBreak;//技能能否被打断，不能则就算前摇期间被异常干扰了，也会照样发动出技能效果，否则就会被打断，没有技能效果

    }
}

