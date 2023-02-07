using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FigureEnumAll
{

}

public enum EPR_AttrBase { hp, tp, restore, atkp, defp, atkm, defm, rapid, move }

public enum EPR_AttrAttach { cooling, defpPierceFix, defmPierceFix, defpPiercePer, defmPiercePer }

public enum EPR_Feature { wind, fire, water, metal, wood, soil, light, dark }

//------------------------------

public enum EPR_SkillTrack { rangeSelf, toArea, dirFirst, dirAll }//���Լ�Ϊ���ĵķ�Χ����ָ�����򡢷����ϵĵ�һ���������ϵ�����

public enum EPR_SkillEffectNature { deviation=1, damage=2, control=4, buff=8 }//λ�ơ��˺������ơ�������

//--------------------------------

public enum EPR_Exception { none, dizzy, disarm, silent, stand, bounce, spring }//ѣ�Ρ���е����Ĭ�����������ɡ�����

public enum EPR_Action { move, still, atk, skill }//�ƶ���������ƽ��������

public enum EPR_Damage { weapon_body, role_skill, weapon_skill }

//--------------------------------

public enum EPR_BuffVaryForm { tmp_instant = 0, tmp_gradual = 1, loss_instant = 2, loss_gradual = 3 }

public enum EPR_BuffVaryMode { fix, percent }

public enum EPR_BuffVaryClass { basis, attach, feature }

//--------------------------------