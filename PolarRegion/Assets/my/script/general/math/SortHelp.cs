
using System.Collections.Generic;
using System;

public class SortHelp//�����㷨��
{
    //��ʾ��ʱ�临�Ӷ�Խ�ߵ��㷨���ɿ��ơ����桢���õĿռ䷴����Խ�󣬲����Ǿ��ò���

    //ƽ��ʱ�临�ӶȴӸߵ��������ǣ�ð������o(n2)����ѡ������o(n2)������������o(n2)����������o(nlogn)����
    //�鲢����o(nlogn)������������o(nlogn)���� ϣ������o(n1.25)������������o(n)��

    //====================================================================

    //ֻ����������Ļ���List�Դ���Sort������[]�䱸��Array.Sort����

    //�����ṩʹ�ð���

    struct Sample : IComparable<Sample>,IComparer<Sample> //ǰһ���ӿڣ��������������Լ��Ƚϣ���һ���ӿڣ��Ǹ����ṩһ���Ƚ�����������ͬ���͵�����������бȽ�
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

    //�����ռ������˼��Ĺ᳹ʵ�֣�û���Ż�

    //IList<int>���������Լ���int[]��List<int>(��Ȼint[]��ToList()��List<int> ��ToArray()

    static void Swap(IList<int> data, int one, int another)
    {
        int tmp = data[one];
        data[one] = data[another];
        data[another] = tmp;
    }

    public static void SelectSort(IList<int> data)//ѡ������
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

    public static void BubbleSort(IList<int> data)//ð������
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

    public static void InsertSort(IList<int> data)//��������
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
