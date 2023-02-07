using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponActionBase : ThrowActionBase, IDamageBring
{//辅助武器本身与人物的联系

    int mFormAtk;

    AttrBaseEquip mAttr;

    public void Ready(AttrBaseEquip attr) 
    {
        ReadyBase();
        mFormAtk = 0;
        mAttr = attr;
    }

    public int AtkForm { get { return mFormAtk; } }

    public MonoBehaviour Mount { get { return this; } }

    public List<AttrBaseEquip> EquipTake { get { return new List<AttrBaseEquip>() { mAttr }; } }//默认自己

    public AttrSkillBody Skill { get { return null; } }//甩动武器，并不携带技能

    public EPR_Damage Nature { get { return EPR_Damage.weapon_body; } }//从武器本身获取时，一定是触碰武器体积了

    public EDamageOpportunity Excite { get { return EDamageOpportunity.only_first; } }//武器一定是一击，不存在其它激发伤害效果的时候

}
