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

public enum EPR_SkillTrack { rangeSelf, toArea, dirFirst, dirAll }//以自己为中心的范围、到指定区域、方向上的第一个、方向上的所有

public enum EPR_SkillEffectNature { deviation=1, damage=2, control=4, buff=8 }//位移、伤害、控制、增减益

//--------------------------------

public enum EPR_Exception { none, dizzy, disarm, silent, stand, bounce, spring }//眩晕、缴械、沉默、禁锢、击飞、弹开

public enum EPR_Action { move, still, atk, skill }//移动、静立、平砍、技能

public enum EPR_Damage { weapon_body, role_skill, weapon_skill }

//--------------------------------

public enum EPR_BuffVaryForm { tmp_instant = 0, tmp_gradual = 1, loss_instant = 2, loss_gradual = 3 }

public enum EPR_BuffVaryMode { fix, percent }

public enum EPR_BuffVaryClass { basis, attach, feature }

//--------------------------------