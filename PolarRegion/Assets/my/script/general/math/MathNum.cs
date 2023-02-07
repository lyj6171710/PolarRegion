using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public static class MathNum
{//calculate

    //============================

    public static float AtLeast0(float result)
    {
        if (result < 0) return 0;
        else return result;
    }

    public static float AtLeast1(float result)
    {
        if (result < 1) return 1;
        else return result;
    }

    public static float AtLeast(float result, float min)
    {
        if (result < min) return min;
        else return result;
    }

    //============================

    public static float CountRemainder(float denominat, float denominator)
    {//求分子除以分母的余数(指超出部分，非还需要才能凑整的部分)，并保留小数与正负
        if (CheckFraction(denominat, denominator) <= 0) 
            return 0;
        else
        {

            bool isContra = IfContra(denominat, denominator);

            denominat = Mathf.Abs(denominat);//先只需考虑绝对值
            denominator = Mathf.Abs(denominator);
            while (denominat >= 0) denominat -= denominator;
            denominat += denominator;//回退到0到分子绝对值之间，大小就是余数了

            if (isContra) return -denominat;
            else return denominat;

        }
    }

    public static int CountTimes(float denominat, float denominator)
    {
        if (CheckFraction(denominat, denominator) <= 0)
            return 0;
        else
        {
            float result = Mathf.Abs(denominat / denominator);
            int round = Mathf.RoundToInt(result);
            float remainder = Mathf.Abs(result - round);//小数部分
            if (remainder <= 2 * Mathf.Epsilon)
                return round;
            else
                return (int)result;
        }
    }

    public static int CountTimes(int denominat, int denominator)
    {
        if (CheckFraction(denominat, denominator) <= 0)
            return 0;
        else
        {
            return Mathf.Abs(denominat / denominator);
        }
    }

    //===================================

    public static float LerpReverse(float cur,float to,float alpha)
    {
        float one = to - cur;
        float two = 1 / Mathf.Abs(one - Mathf.Sqrt(one));
        float three = two * alpha;
        float four = one * three;
        return cur + four;
    }

    //===================================

    public static float MapClamp(float cur, float min, float max, float from, float to)
    {//请确保cur在min和max之间，否则结果未知
        float ratio;
        if (max >= min)
            ratio = (cur - min) / (max - min);
        else//当最小值大于最大值，越接近最大值，越返回from
            ratio = (min - cur) / (min - max);
        return (to - from) * ratio;
    }

    //===================================

    public static int FloorToIntAndZero(float need)
    {
        if (need >= 0) return Mathf.FloorToInt(need);
        else return Mathf.CeilToInt(need);
    }

    public static int FloatToClosestInt(float need)
    {
        return Mathf.RoundToInt(need);
    }

    //========================================

    static int CheckFraction(float denominat, float denominator)//苛刻情况才会过不了
    {
        if (denominat == 0)
            return 0;//要注意该特殊情况
        else if (denominator == 0)
        {
            Debug.Log("分母不应该为零");
            return -1;//先可排除分子分母为零的情况
        }
        else
            return 1;
    }

    public static bool IfContra(float denominat, float denominator)//是否相异
    {
        if (denominat == 0) return false;//分子为零时，视为与分母同向
        else if (denominat > 0 && denominator > 0) return false;//省性能用的，频率最高
        else if (denominat > 0 && denominator < 0) return true;
        else if (denominat < 0 && denominator > 0) return true;
        else return false;
    }

    public static bool IfContra(int denominat, int denominator)//是否相异
    {
        if (denominat == 0) return false;//分子为零时，视为与分母同向
        else if (denominat > 0 && denominator > 0) return false;//省性能用的，频率最高
        else if (denominat > 0 && denominator < 0) return true;
        else if (denominat < 0 && denominator > 0) return true;
        else return false;
    }

    //========================================

    //The well-known 3.14159265358979... value(Read Only).
    public const float PI = (float)Math.PI;

    //A representation of positive infinity (Read Only).
    public const float Infinity = float.PositiveInfinity;

    //A representation of negative infinity (Read Only).
    public const float NegativeInfinity = float.NegativeInfinity;

    //Degrees-to-radians conversion constant (Read Only).
    public const float Deg2Rad = (float)Math.PI / 180f;

    // Radians-to-degrees conversion constant (Read Only).
    public const float Rad2Deg = 57.29578f;//

    //A tiny floating point value (Read Only).
    //高精度
    public static readonly float Epsilon = UnityEngineInternal.MathfInternal.IsFlushToZeroEnabled ?
        UnityEngineInternal.MathfInternal.FloatMinNormal :
        UnityEngineInternal.MathfInternal.FloatMinDenormal;

    //低精度
    public const float cFloat0 = 0.001f;

    public static bool IsNear0(float f)
    {
        if (Abs(f) < cFloat0) return true;
        else return false;
    }

    public static float Min(float a, float b)
    {
        return (a < b) ? a : b;
    }

    public static float Min(params float[] values)
    {
        int num = values.Length;
        if (num == 0)
        {
            return 0f;
        }

        float num2 = values[0];
        for (int i = 1; i < num; i++)
        {
            if (values[i] < num2)
            {
                num2 = values[i];
            }
        }

        return num2;
    }

    public static int Min(int a, int b)
    {
        return (a < b) ? a : b;
    }

    public static int Min(params int[] values)
    {
        int num = values.Length;
        if (num == 0)
        {
            return 0;
        }

        int num2 = values[0];
        for (int i = 1; i < num; i++)
        {
            if (values[i] < num2)
            {
                num2 = values[i];
            }
        }

        return num2;
    }

    public static float Max(float a, float b)
    {
        return (a > b) ? a : b;
    }

    public static float Max(params float[] values)
    {
        int num = values.Length;
        if (num == 0)
        {
            return 0f;
        }

        float num2 = values[0];
        for (int i = 1; i < num; i++)
        {
            if (values[i] > num2)
            {
                num2 = values[i];
            }
        }

        return num2;
    }

    public static int Max(int a, int b)
    {
        return (a > b) ? a : b;
    }

    public static int Max(params int[] values)
    {
        int num = values.Length;
        if (num == 0)
        {
            return 0;
        }

        int num2 = values[0];
        for (int i = 1; i < num; i++)
        {
            if (values[i] > num2)
            {
                num2 = values[i];
            }
        }

        return num2;
    }

    public static int Sign1(float f)
    {
        return (f >= 0f) ? 1 : -1;
    }

    public static int Sign10(float f)
    {
        if (IsNear0(f)) return 0;
        else return Sign1(f);
    }

    public static float Abs(float f)
    {
        return (float)Math.Abs(f);//系统本身可能对该计算有优化
        //return f >= 0 ? f : -f;
    }

    public static int Abs(int i)
    {
        return Math.Abs(i);
    }

    public static float Clamp(float value, float min, float max)
    {
        if (value < min)
        {
            value = min;
        }
        else if (value > max)
        {
            value = max;
        }

        return value;
    }

    public static int Clamp(int value, int min, int max)
    {//value可以抵达边缘值
        if (value < min)
        {
            value = min;
        }
        else if (value > max)
        {
            value = max;
        }

        return value;
    }

    public static float Clamp01(float value)
    {//Clamps value between 0 and 1 and returns value.
        if (value < 0f)
        {
            return 0f;
        }

        if (value > 1f)
        {
            return 1f;
        }

        return value;
    }

    public static float Lerp(float a, float b, float t)
    {//线性插值，如果需要达到t值，需要一个逐渐变大的t值。
     //另外可以将a设置为当前结果，t设置为一个恒定值，这样会得到一个逐渐减速的值，最后结果会无限接近于b
        return a + (b - a) * Clamp01(t);
    }

    public static float LerpUnclamped(float a, float b, float t)
    {//Linearly interpolates between a and b by t with no limit to t.
        return a + (b - a) * t;
    }

    public static float MoveTowards(float current, float target, float maxDelta)
    {//Moves a value current towards target.
     //通过只变化delta，可线性变化
        if (Math.Abs(target - current) <= maxDelta)
        {
            return target;
        }

        return current + Sign1(target - current) * maxDelta;
    }

    public static float SmoothStep(float from, float to, float t)
    {//Interpolates between min and max with smoothing at the limits.
     //和Lerp一样的处理方式，只是变化曲线是一个先加速后减速的结果
        t = Clamp01(t);
        t = -2f * t * t * t + 3f * t * t;
        return to * t + from * (1f - t);
    }

    public static float Gamma(float value, float absmax, float gamma)
    {//好像和图像颜色有关
        bool flag = value < 0f;
        float num = Math.Abs(value);
        if (num > absmax)
        {
            return flag ? (0f - num) : num;
        }

        float num2 = (float)Math.Pow(num / absmax, gamma) * absmax;
        return flag ? (0f - num2) : num2;
    }

    public static bool Approximately(float a, float b)
    {//Compares two floating point values and returns true if they are similar.
        return Math.Abs(b - a) < Max(1E-06f * Max(Math.Abs(a), Math.Abs(b)), Epsilon * 8f);
    }

    public static float Repeat(float t, float length)
    {//循环值t，使其不大于长度，也不小于0。
        return Clamp(t - (float)Math.Floor(t / length) * length, 0f, length);
    }

    public static float PingPong(float t, float length)
    {//PingPong returns a value that will increment and decrement between the value 0 and length.
        t = Repeat(t, length * 2f);
        return length - Math.Abs(t - length);
    }

    public static float InverseLerp(float a, float b, float value)
    {//Calculates the linear parameter t that produces the interpolant value within the range [a, b].
        if (a != b)
        {
            return Clamp01((value - a) / (b - a));
        }

        return 0f;
    }

}
