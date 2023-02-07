using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EKindShowForm { EveryCopy, OnlyAimOne, ScreenMid }

public enum ERPGUnifiedFeature//统一指代RPG游戏中的特性
{
    Grade, Health, Exert, Heal, Currency,
    Prop, Technique, Degree, Score, Equip,
    Defense, Attack, Exact, Evade, Luck,
    Retreat, Status, Purchase, Sell
}

public enum ERPGUnifiedScene//统一指代某一类情景
{
    NumHave, ExpAcc, ExpNeedToNextLv,
    TipForSave, TipForLoad, PartyCall,
    EnemyEmerge, Evacuate, BeatOut, Defeat,
    GetFrom, Upgrade, Learned, MakeUseOf,
    DeathBlow, CriticalHit, GetHurt, GetHit,
    NumIncrease, NumDecrease, RaiseBy, LowerBy,
    NoImpact, ExactHit, EvadeFrom, StrikeBack,
    ProtectFor, RelieveBy, GetRidOf, ActionFail,
    GetRecovery, WinVictory, InBattle, AssaultFrom
}

public enum ERPGCoverSubKind
{
    Nature,Technique,Weapon,Armor,Equip
}

public enum EKindAtkConstrain {
    OnlyOne,OnlyCanAtk
}

public enum EKindRelieveOccasion {
    BattleEnd,SufferConstrain,NextTurn,NextHpDown,NumStep
}

public enum EKindRoleStatus {  }

public enum EKindWeapon { Sword, Knife }

public enum EKindArmor { 物理小型,物理大型,魔法小型,魔法大型}

public enum EKindEquip { weapon,armor}

public enum EKindBodyComp { head,hand,arm}

public enum EKindProp { 普通,贵重,隐藏 }

public enum ECampSelect { 我方,敌方,中立,任一方 }

public enum EKindTechnique { 魔法,技巧,必杀技 }

public enum ENumTakeFor { 单体,全体,随机 }

public enum EStateLive { 存活,死亡,无条件 }

public enum EKindAtkAction { 突刺, 挥舞, 投掷 }

public enum EKindConditionCanUse { 不能,随时,战斗,菜单 }

public enum EKindAtkFeature { hp伤害,hp回复,sp伤害}

public enum EKindHitNature{ 物理,魔法,元素}

public enum EKindNature { 金,木,水,火,土}

public class DataMaterial {
    
}
