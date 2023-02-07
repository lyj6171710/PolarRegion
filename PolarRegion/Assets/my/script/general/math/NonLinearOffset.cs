using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class NonLinearOffset : UnitOffset
{//可辅助任何二维属性的非线性变化，支持二维上的一维数值(另一维数值不变)

    //这里建议，转换成线性变化，不要用非线性变化来实现效果
}
