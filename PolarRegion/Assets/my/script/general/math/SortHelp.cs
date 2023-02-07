
using System.Collections.Generic;
using System;

public class SortHelp//排序算法集
{
    //提示：时间复杂度越高的算法，可控制、干涉、利用的空间反而会越大，并不是就用不到

    //平均时间复杂度从高到低依次是：冒泡排序（o(n2)），选择排序（o(n2)），插入排序（o(n2)），堆排序（o(nlogn)），
    //归并排序（o(nlogn)），快速排序（o(nlogn)）， 希尔排序（o(n1.25)），基数排序（o(n)）

    //====================================================================

    //只是排序需求的话，List自带有Sort方法，[]配备有Array.Sort方法

    //这里提供使用案例

    struct Sample : IComparable<Sample>,IComparer<Sample> //前一个接口，是其它事物与自己比较，后一个接口，是该类提供一个比较器，帮助对同类型的两个事物进行比较
    {
        public int id;

        public Sample(int num) { id = num; }

        public int Compare(Sample x, Sample y)//(1)
        {
            return y.id - x.id;
        }

        public int CompareTo(Sample other)
        {
            return other.id - id;
        }
    }

    static int CompareWith(Sample a, Sample b)//(2)
    {
        if (b.id > a.id) return 1; else if (b.id == a.id) return 0; else return -1;
    }

    static void SortSample()
    {
        Sample[] samples = new Sample[3] { new Sample(1), new Sample(2), new Sample(3) };

        Array.Sort(samples, (a, b) => { if (b.id > a.id) return 1; else if (b.id == a.id) return 0; else return -1; });//(3)

        Array.Sort(samples, CompareWith);

        Array.Sort(samples, (a, b) => { return a.CompareTo(b); });

        Array.Sort(samples, new Sample());

        List<Sample> samps = new List<Sample>();
        samps.Sort((a, b) => { if (b.id > a.id) return 1; else if (b.id == a.id) return 0; else return -1; });
    }

    //===================================================================

    //这里收集最基本思想的贯彻实现，没有优化

    //IList<int>，这样可以兼容int[]和List<int>(虽然int[]有ToList()，List<int> 有ToArray()

    static void Swap(IList<int> data, int one, int another)
    {
        int tmp = data[one];
        data[one] = data[another];
        data[another] = tmp;
    }

    public static void SelectSort(IList<int> data)//选择排序
    {
        for (int i = 0; i < data.Count - 1; i++)
        {
            int min = i;
            int temp = data[i];
            for (int j = i + 1; j < data.Count; j++)
            {
                if (data[j] < temp)
                {
                    min = j;
                    temp = data[j];
                }
            }
            if (min != i)
                Swap(data, min, i);
        }
    }

    public static void BubbleSort(IList<int> data)//冒泡排序
    {
        for (int i = data.Count - 1; i > 0; i--)
        {
            for (int j = 0; j < i; j++)
            {
                if (data[j] > data[j + 1])
                    Swap(data, j, j + 1);
            }
        }
    }

    public static void InsertSort(IList<int> data)//插入排序
    {
        int temp;
        for (int i = 1; i < data.Count; i++)
        {
            temp = data[i];
            for (int j = i - 1; j >= 0; j--)
            {
                if (data[j] > temp)
                {
                    data[j + 1] = data[j];
                    if (j == 0)
                    {
                        data[0] = temp;
                        break;
                    }
                }
                else
                {
                    data[j + 1] = temp;
                    break;
                }
            }
        }
    }

    //===============================================================================

}
