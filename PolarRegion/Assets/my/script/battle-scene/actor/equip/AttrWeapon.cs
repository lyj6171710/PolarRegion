using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttrWeapon : AttrBaseEquip
{
    public bool whip = true;//远程武器还是近程武器
    public float range = 0.5f;//攻击距离
    public int angleSinceRight;//图像有伤害的一边，相对正右侧的方向
}
