using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//搜集精髓：主题定好：主题的名称要规范，主题的意义能解释清

public struct TitleCover
{
    AudioClip theme;
    Sprite background;
}

public struct TileCover
{
    EToward4[] dirBan;//四方向
    int stairCan;
    int density;
    bool isCounter;//柜台
    bool harmful;//负面地形，有害地形
    int firstLay;//放置时会优先处于哪一层
}

public struct TieProperties
{
    float hp ;//血量
    float sp ;//气力
    float restore ;//气力回复
    float atkp ;//物理攻击力
    float defp ;//物理防御力
    float atkm ;//魔法攻击力
    float defm ;//魔法防御力
    float agile;//敏捷
    float luck;//幸运
}

public class WeaponCover : EquipCover
{
    EKindWeapon kind;
}

public class ArmorCover:EquipCover
{
    EKindArmor kind;
    EKindBodyComp comp;
}

public class EquipCover:ItemCover
{
    TieProperties add;
    BuffFeature buff;
}

public class ItemCover:LooseCover
{
    float price;
}

public class LooseCover
{
    string name;
    string explain;
}

public class PropCover:ItemCover
{
    bool willConsume;
    EKindProp kind;
    ECamp useFor;
    ENumTakeFor range;
    EKindConditionCanUse condition;
    float hpAdd;
    float spAdd;
    string statusAdd;
    string statusRemove;
    float ratioSuccess;
    string animSelect;
}

public class VocationCover : LooseCover
{
    AnimationCurve curve;
    Dictionary<int, string> WhenLvLeanredTech;
    BuffFeature buff;
}

public class RoleCover : LooseCover
{
    VocationCover vocation;
    int lvStart;
    int lvMax;
    Sprite face;
    List<EquipCover> equipsStart;
    BuffFeature buff;
}

public class TechniqueCover:LooseCover
{
    EKindTechnique kind;
    float spCost;
    ENumTakeFor useFor;
    EKindConditionCanUse condition;
    int times;//效果次数
    EKindHitNature natureHit;
    string animSelect;
    string weaponAsk;

    EKindAtkFeature atkFeature;
    EKindNature atkNature;
    string formula;//计算公式
    float dispersity;//伤害浮动
    bool canCritial;//是否能暴击
}

public struct FoeCover
{
    TieProperties properties;
    float expCanGet;
    float moneyCanGet;
    List<string> propMayHave;
    List<EKindAct> actionMode;//行动模式，比如第一回合做什么，第二回合做什么
    BuffFeature buffNative;
}

public struct CharacterStatus
{
    string name;
    EKindAtkConstrain constrainAtk;
    EKindRelieveOccasion relieveWhen;
    int prior;
}

public enum EKindTieAbility { hp,phyiscalAtk,magicAtk,luck,agi}
public enum EKindSimpleAbility { hpHeal,hitChance}
public enum EKindCashOutAbility { physicalDamage,magicDamage}

public class BuffFeature
{
    float NatureValidDegree;
    float StatusValidDegree;
    EKindRoleStatus StatusImmunity;

    float rateAbility;
    
    float rateDetailAbility;
}

public struct AnimCover
{
    string name;
    EKindWeapon kindFit;
    EKindShowForm playForm;
    List<AnimFrameSet> anim;
    float speed;
    Dictionary<int, AudioClip> audio;
    Dictionary<int, AnimFlash> flash;

    public struct AnimFrameSet
    {
        Sprite pic;
        Vector2 scale;
        Vector2 offset;
        float angle;
    }

    public struct AnimAudio
    {
        int frameAt;
    }

    public struct AnimFlash
    {
        float keepTime;
        Color color;
    }
}

public class PREntityInstance {

    public struct SceneMatch
    {
        string speech;
        AudioClip audio;
    }
    
    Dictionary<ERPGUnifiedScene, SceneMatch> represent =
        new Dictionary<ERPGUnifiedScene, SceneMatch>();
    
    public class WeaponHoldAnim
    {
        public EKindWeapon weapon;
        public EKindAtkAction motion;
        Dictionary<EKindWeapon, Sprite> diagram;
    }

    public const string MoneyUnit = "G";

    
}
