using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultEnum : PropertyAttribute {//枚举变量，可多选化的特性

    public bool IfPick(Enum enumAsk, Enum enumState)
    {// 判断是否选中该枚举值
        int index = 1 << enumAsk.GetHashCode();//将1左移
        int result = enumState.GetHashCode();//int只是起存储空间的作用
        if ((result & index) == index)
        {
            return true;
        }
        return false;
    }

    int GetEnumInt(Enum type)//获取枚举值对应的数值
    {//该函数只是如何得到对应数值的示例，外界可以直接照做(如果转换过程简单时)
        return type.GetHashCode();
    }
}

public enum EVoid { o }//可用来表示空值，强调不需要值

public enum ENum { one, two, three }

public enum EToward4 { up = 0, down = 1, left = 2, right = 3, middle = 4 }//必需要middle，因为可能哪个方向都不是，就得用mid来区别
public enum EToward8 { up = 0, down = 1, left = 2, right = 3, middle = 4, upLeft = 5, upRight = 6, downLeft = 7, downRight = 8 }
public enum ETowardX4 { upLeft = 5, upRight = 6, downLeft = 7, downRight = 8 , middle = 9 }
public enum EFormOffset { percent, fix, to }//以，百分比/量长，描述需要的变化

public enum EKindAct { constant, accelerate, lerp }//怎么变化的

public enum EDrift { none, horizontal, vertical, oblique }//竖着、横着、斜着，主要用于应付零值时的特殊情况

public enum EMsgs //申明那些会有相互需求的信息，更适合频繁或普遍，必定需要的那些信息
{ whether_alive_ = 0, whether_trigger_ = 1, audio_effect = 2, footfall_effect = 3, whether_ready_ = 4, tell_damage_ = 5 }

public enum EVarKind { boolean = 0, integer = 1, fraction = 2 , char_queue=3 }

public enum ECamp { amity, enemy, neutral }

public enum EMotion { constant, accelerate, decelerate, up_peak, down_peak }//一般移动模式

public enum EAngle { degree, radian, vector }