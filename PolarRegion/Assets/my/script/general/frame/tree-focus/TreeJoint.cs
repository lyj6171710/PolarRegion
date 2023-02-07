using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeJoint : TreeFocus//承上启下，那些既作为分支，又作为中心的部门
{
    public TreeBranch mFocusBranch;//这个由分支赋值给自己
    //作为局部核心的自己，可能只是更上一级分布的一个部门，这里获取这个部门，方便控制
}
