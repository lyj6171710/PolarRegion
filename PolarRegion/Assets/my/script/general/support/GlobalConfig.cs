using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalConfig : MonoBehaviour
{
    public const int prior_ui = 9;
    public const int pixel_per_unit = 450;//要让游戏用到的画布，属性上都贴近这几个参照值

    public const int order_background = -2;
    public const int order_land = -1;
    public const int order_shadow = 0;
    public const int order_actor = 1080;//1到1080
    public const int order_foreground = 1100;

    public const string layer_foot = "foot";//绊脚石
    public const string layer_actor = "actor";//动物
    public const string layer_atk = "atk";//伤害
    public const string layer_still = "still";//静物
    public const string layer_back = "ground";//地面，背景

    public const string sign_amity = "a";//玩家方必然应是amity方
    public const string sign_enemy = "b";
}
