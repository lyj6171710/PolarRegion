using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillActionBase : ThrowActionBase, IDamageBring
{//辅助技能子弹与人物本身的联系

    int mFormAtk;

    FigureProfile mRole;

    AttrSkillBody mAttr;

    public void Ready(AttrSkillBody attr, FigureProfile role)
    {
        ReadyBase();
        mAttr = attr;
        mRole = role;
        mFormAtk = 0;
        
    }

    public int AtkForm { get { return mFormAtk; } }

    public MonoBehaviour Mount { get { return this; } }

    public List<AttrBaseEquip> EquipTake { get { return mRole.RequestEquips(); } }//默认所有装备

    public AttrSkillBody Skill { get { return mAttr; } }

    public EPR_Damage Nature { get { return EPR_Damage.role_skill; } }//从技能本身获取时，一定是触碰技能子弹了

    public EDamageOpportunity Excite { get { return mAttr.period; } }

    //=============================


}
