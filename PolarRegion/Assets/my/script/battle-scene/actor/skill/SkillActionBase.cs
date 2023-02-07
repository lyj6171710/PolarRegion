using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillActionBase : ThrowActionBase, IDamageBring
{//���������ӵ������ﱾ�����ϵ

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

    public List<AttrBaseEquip> EquipTake { get { return mRole.RequestEquips(); } }//Ĭ������װ��

    public AttrSkillBody Skill { get { return mAttr; } }

    public EPR_Damage Nature { get { return EPR_Damage.role_skill; } }//�Ӽ��ܱ����ȡʱ��һ���Ǵ��������ӵ���

    public EDamageOpportunity Excite { get { return mAttr.period; } }

    //=============================


}
