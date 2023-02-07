using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Probability
{

    public static float RandomValue(float floor, float ceiling)
    {
        ceiling = (ceiling >= floor) ? ceiling : floor;
        return UnityEngine.Random.Range(floor, ceiling);
    }

    public static int RandomValue(int floor, int ceiling)//考虑为闭区间，且所有这里的方法，也都面向闭区间来考虑的
    {
        ceiling = (ceiling >= floor) ? ceiling : floor;//顶板理应高于地板
        return UnityEngine.Random.Range(floor, ceiling + 1);//参数为整型时，这个内置函数不包含右端那个数
    }

    //==================================================

    public static bool WhetherHitWhen(float denominat, float denominator)
    {
        denominat = denominat < 0 ? 0 : denominat;
        denominator = denominator < 1 ? 1 : denominator;
        denominat = denominat > denominator ? denominator : denominat;
        float value = RandomValue(0, denominator);
        if (value >= 0 && value <= denominat)
            return true;
        else
            return false;
    }

    //==================================================

    public static int[] RandomValueEquable(int num, int floor, int ceiling)//返回的多个随机值，可能存在等值情况
    {
        if (num <= 0 || floor > ceiling)
            return null;
        else
        {
            int[] result = new int[num];
            for (int i = 0; i < num; i++)
                result[i] = RandomValue(floor, ceiling);
            return result;
        }
    }

    public static int[] RandomValueMutex(int num, int floor, int ceiling)
    {
        if (num < 0 || floor > ceiling)
            return null;
        else 
        {
            int numCanPick = ceiling - floor + 1;
            if (num <= numCanPick)//所需随机数的数量小于差距值时，才能给出相互互斥的随机数
            {
                int[] result = new int[num];
                for (int i = 0; i < num; i++)
                {
                    int acc = 0;//计数，累积发生“需要重新随机”的次数
                    do
                    {
                        result[i] = RandomValue(floor, ceiling);
                        if (++acc > HowTolerate(numCanPick, i)) 
                        { 
                            Debug.Log("难以互斥"); 
                            break;
                        };//防止某种失误，导致这里死循环
                    } while (IfAlreadyExist(result, i));//如果已经有，重新随机
                }
                return result;
            }
            else
                return ReportError();
        }
    }

    //==================================================

    public static int[] RandomValueEquable(int num, int[] optional)//从所给的几个数中，随机选出几个数
    {
        int numCanPick = optional.Length;
        if (num < 1 || numCanPick < 1) 
            return ReportError();
        else
        {
            int[] result = new int[num];
            for (int i = 0; i < num; i++)
                result[i] = optional[Mathf.RoundToInt(RandomValue(0.0f, 1.0f) * (numCanPick - 1))];
            return result;
        }
    }

    public static int[] RandomValueMutex(int num, int[] optional)
    {
        int numCanPick = optional.Length;
        if (num < 1 || numCanPick < 1 || num > numCanPick)
            return ReportError();
        else
        {
            int[] result = new int[num];
            for (int i = 0; i < num; i++)
            {
                int acc = 0;
                do
                {
                    result[i] = optional[Mathf.RoundToInt(RandomValue(0.0f, 1.0f) * (numCanPick - 1))];
                    if (++acc > HowTolerate(numCanPick, i))
                    { Debug.Log("难以互斥"); break; }
                } while (IfAlreadyExist(result, i));
            }
            return result;
        }
    }

    public static int[] RandomValueMutex(int num, int[] optional, int[] except)//对所给数据集，会按所给的另一数据集排除其中所含元素，再随机
    {
        if (except == null || except.Length == 0) return ReportError();

        int curFill = 0;
        int[] load = new int[optional.Length];
        for (int i = 0; i < optional.Length; i++)
        {
            if (!IfExist(except, optional[i])) //数据集中
            {
                load[curFill] = optional[i];
                curFill++;
            }
        }
        int[] filter = new int[curFill];
        for (int i = 0; i < filter.Length; i++)
        {
            filter[i] = load[i];
        }
        return RandomValueMutex(num, filter);
    }

    //==================================================

    public static int[] RandomValueMutex(int num, List<Vector2Int> rangesRefer)//当所给参数范围，相互间互斥时，才会有所返回随机数也互斥的效果
    {
        int areaNum = rangesRefer.Count;
        if (num < 1 || areaNum < 1)
            return ReportError();
        else
        {
            List<Vector2Int> ranges = new List<Vector2Int>();//这里防止把外界数据给修改了
            foreach (Vector2Int range in rangesRefer) { ranges.Add(range); }

            for (int i = 0; i < areaNum; i++)//剔除不合法的范围指示(不让外界自己判断，因为这样有利于共通化处理)
            {
                if (ranges[i].x > ranges[i].y)
                {
                    ranges.RemoveAt(i);
                    areaNum--;
                    i--;
                }
            }
            if (areaNum < 1)
                return ReportError();
            else if (areaNum == 1)
                return RandomValueMutex(num, ranges[0].x, ranges[0].y);
            else //areaNum >= 2
            {
                int totalNum = GetTotalAmount(ranges);
                if (num > totalNum) return ReportError();
                int[] select = RandomValueMutex(num, 0, totalNum - 1);//抽序数
                int[] result = new int[num];
                for (int i = 0; i < num; i++)
                {
                    result[i] = PickOne(select[i], ranges);
                }
                return result;
            }
        }
    }

    static int GetTotalAmount(List<Vector2Int> ranges)//若干个范围所包含有的单元的个数
    {
        int total = 0;
        for (int i = 0; i < ranges.Count; i++)//合并
        {
            total += ranges[i].y - ranges[i].x + 1;
        }
        return total;
    }

    static int PickOne(int index, List<Vector2Int> ranges)//index指的是范围内的第几个值(且假设这些范围，前后相接，合并到一起时)
    {
        int lay = 0;
        int span = ranges[lay].y - ranges[lay].x;
        while (index > span)
        {
            index -= (span + 1);
            lay++;
            span = ranges[lay].y - ranges[lay].x;
        }
        return ranges[lay].x + index;
    }

    //内部工具========================================

    static float HowTolerate(int total,int pick)
    {
        return (10 + 4.0f * total) / (total - pick);
        //10是设置的一个底限，因为总量低时，总是容易重复的，容忍度也得增高
        //需求数值量越多，重复率会越高，容忍度就需要随之变高
        //可选数值量越多，重复率越低，但需求数值量增加到接近可选数据量的时候，重复率陡然增高
    }

    static bool IfExist(int[] check, int need)
    {
        for (int i = 0; i < check.Length; i++)
        {
            if (check[i] == need)
            {
                return true;
            }
        }
        return false;
    }

    static bool IfAlreadyExist(int[] check,int compareIndex)//从指定序数往前比较
    {
        if (compareIndex < 0 || compareIndex >= check.Length)
        {
            Debug.Log("参数错误");
            return false;
        }
        else if (compareIndex == 0)
        {
            return false;//只有它自己，肯定没有重复的了
        }
        else
        {
            for (int i = compareIndex - 1; i > -1; i--) 
            {
                if (check[compareIndex] == check[i])
                {
                    return true;
                }
            }
            return false;
        }
    }

    static int[] ReportError()
    {
        Debug.Log("生成随机数失败");
        return null;
    }

    //============================================

    public static int RandomSign()
    {
        int rdm = UnityEngine.Random.Range(0, 2);//从[0，2)中抽取整数，因此或0或1
        return (rdm == 0 ? 1 : 0) - rdm;
    }

    public static Vector2 RandomPosOnCircle(float radius, Vector2 center)
    {
        float radian = UnityEngine.Random.Range(0, Mathf.PI * 2);
        Vector2 offsetXY = new Vector2(Mathf.Sin(radian) * radius, Mathf.Cos(radian) * radius);
        return center + offsetXY;
    }

    public static Vector3 RandomPosOnSphere(float radius, Vector3 center)
    {
        Vector2 pickLeft = RandomPosOnCircle(radius, Vector2.zero);
        Vector2 pickRight = RandomPosOnCircle(pickLeft.y, Vector2.zero);
        Vector3 offsetXYZ = new Vector3(pickLeft.x, pickRight.x, pickRight.y);
        return center + offsetXYZ;
    }

    public static float SelectFloat(bool from, float ifTrue, float ifFalse)
    {
        float one = (from ? 1 : 0) * ifTrue;
        float two = (from ? 0 : 1) * ifFalse;

        return one + two;

        //上面这种方式是针对那种缺乏流程控制功能的情况，以至于有同样的效果
        //当有流程控制功能时，直接可以按以下方式实现选择，无需特别处理
        //return from ? ifTrue : ifFalse;
    }

}