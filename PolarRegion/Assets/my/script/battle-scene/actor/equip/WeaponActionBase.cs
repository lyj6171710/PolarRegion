using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponActionBase : ThrowActionBase, IDamageBring
{//���������������������ϵ

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

    public List<AttrBaseEquip> EquipTake { get { return new List<AttrBaseEquip>() { mAttr }; } }//Ĭ���Լ�

    public AttrSkillBody Skill { get { return null; } }//˦������������Я������

    public EPR_Damage Nature { get { return EPR_Damage.weapon_body; } }//�����������ȡʱ��һ���Ǵ������������

    public EDamageOpportunity Excite { get { return EDamageOpportunity.only_first; } }//����һ����һ�������������������˺�Ч����ʱ��

}
