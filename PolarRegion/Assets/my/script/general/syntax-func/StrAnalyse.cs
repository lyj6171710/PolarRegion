using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;

public static class StrAnalyse
{

    public static bool IsSupport(string[] socket, string[] plug)//ǰ�߰�������ȫ��ʱ��������
    {
        if (plug.Length > socket.Length) return false;
        
        int match = plug.Length;
        for (int i = 0; i < plug.Length; i++)
        {
            for (int j = 0; j < socket.Length; j++)
            {
                if (plug[i] == socket[j])
                {
                    match--;
                }
                if (match == 0) break;
            }
        }
        if (match == 0) return true;
        else return false;
    }

    public static bool HaveSame(string[] group, string match)//ǰ�ߺ��к���ʱ��������
    {
        foreach (string s in group)
        {
            if (s == match) return true;
        }
        return false;
    }

    public static bool HaveSame(string[] group, string[] others)//ǰ�ߺ��к���֮һʱ��������
    {
        foreach (string s in others)
        {
            foreach (string s2 in group)
            {
                if (s == s2) return true;
            }
        }
        return false;
    }

    //=================================

    public static List<string> FilterStrings(string[] source, string pattern, RegexOptions options = RegexOptions.None)
    {
        List<string> result = new List<string>();

        Regex regex = new Regex(RegexHelp.Wrap(pattern), options);
        foreach (string simple in source)
        {
            if (regex.IsMatch(simple))
                result.Add(simple);
        }
        return result;
    }

    public static List<string> FilterStrings(string[] source, string[] contains)
    {
        List<string> result = new List<string>();

        foreach (string simple in source)
        {
            bool cover = true;
            foreach (string elem in contains) 
                if (!simple.Contains(elem)) 
                    cover = false;
            if(cover) result.Add(simple);
        }

        return result;
    }

    //=================================

    public static string TrimPrefix(string source, string prefix)
    {
        return source.Substring(prefix.Length);
    }

    //---------------------------

    public static string[] SplitBy(string source, char separator)
    {
        return source.Split(separator);
    }

    //---------------------------

    public static string GetPrefix(string source, char divide)
    {
        int splitAt;
        return GetSlitFromLeft(source, divide, out splitAt);
    }

    public static string[] GetBothSides(string source, char divide)
    {
        int splitAt;
        string left = GetSlitFromLeft(source, divide, out splitAt);
        if (splitAt < 0)
            return new string[] { source, "" };
        else
        {
            string right = source.Substring(splitAt + 1);
            return new string[] { left, right };
        }
    }

    static string GetSlitFromLeft(string source, char divide, out int splitAt)
    {
        splitAt = source.IndexOf(divide);
        if (splitAt < 0)
            return source;
        else
            return source.Substring(0, splitAt);
    }

    public static string GetPrefixFromRight(string source, char divide)
    {
        int splitAt;
        return GetSlitFromRight(source, divide, out splitAt);
    }

    static string GetSlitFromRight(string source, char divide, out int splitAt)
    {
        splitAt = source.LastIndexOf(divide);
        if (splitAt < 0)
            return source;
        else
            return source.Substring(0, splitAt);
    }

    //-----------------------------

    public static string GetSuffix(string source, char divide)
    {
        int splitAt = source.LastIndexOf(divide);
        if (splitAt < 0)
            return source;
        else
            return source.Substring(splitAt + 1, source.Length - (splitAt + 1));
    }

    //=================================

    public static int[] PickNum(string source)//���ַ�������ȡ����
    {
        var matches = Regex.Matches(source, @"(\d+)");
        int[] nums = new int[matches.Count];
        for (int i = 0; i < matches.Count; i++)
        {
            nums[i] = int.Parse(matches[i].Value);
        }
        return nums;
    }

    //=================================

    static void Sample()
    {
        /*
        ToLower()    //תΪСд�ַ���"AbC"-->"abc"
        ToUpper()    //תΪ��д"AbC" -->"ABC"
        Trim()       //ȥ���ַ�����β�Ŀո�"  abc "-->"abc"
        Equals(string value,StringComparison comparisonType);     //����ж�

        CompareTo(string value)             //��value�Ƚϴ�С

        Split(params char [] separator)     //separator �Ƿָ��ַ����磺','��'|' �ȵȡ�    
        Split(char [] separator ,StringSplitOptions  splitOpt)//StringSplitOptions.RemoveEmptyEntries  
        Split(string[] separator,StringSplitOptions splitOpt)// ���ַ����ָ�

        Replace(char oldChar,char newChar)  //�滻�ַ����е��ַ����磺'a'�滻Ϊ'b'
        Replace(string oldStr,string newStr)//�滻�ַ����е��ַ������磺"��ʱ��"�滻Ϊ"������"

        SubString(int startIndex)            //��ָ����ſ�ʼ,һֱ�����,��ɵ��ַ���
        SubString(int startIndex,int length) //��ָ�����startIndex,����ȡlength��������������Ȼᱨ�쳣

        Contains(char c)      // �Ƿ���� �ַ�
        Contains(string str)  // �Ƿ���� ���ַ���

        StartsWith(string str) //�Ƿ���str��ͷ,�磺http://baidu.com����http://��ͷ
        EndsWith(string str)   //�Ƿ���str��β

        IndexOf(char c)        //�ҵ���һ���ַ�c��index�����û�ҵ�����-1
        IndexOf(string str)    //�ҵ���һ���ַ���str��λ��
         */

        /*
        1	public static int Compare( string strA, string strB )�Ƚ�����ָ���� string ���󣬲�����һ����ʾ����������˳�������λ�õ��������÷������ִ�Сд��
        2	public static int Compare( string strA, string strB, bool ignoreCase )�Ƚ�����ָ���� string ���󣬲�����һ����ʾ����������˳�������λ�õ����������ǣ������������Ϊ��ʱ���÷��������ִ�Сд��
        3	public static string Concat( string str0, string str1 )�������� string ����
        4	public static string Concat( string str0, string str1, string str2 )�������� string ����
        5	public static string Concat( string str0, string str1, string str2, string str3 )�����ĸ� string ����
        6	public bool Contains( string value )����һ����ʾָ�� string �����Ƿ�������ַ����е�ֵ��
        7	public static string Copy( string str )����һ����ָ���ַ���������ֵͬ���µ� String ����
        8	public void CopyTo( int sourceIndex, char[] destination, int destinationIndex, int count )�� string �����ָ��λ�ÿ�ʼ����ָ���������ַ��� Unicode �ַ������е�ָ��λ�á�
        9	public bool EndsWith( string value )�ж� string ����Ľ�β�Ƿ�ƥ��ָ�����ַ�����
        10	public bool Equals( string value )�жϵ�ǰ�� string �����Ƿ���ָ���� string ���������ͬ��ֵ��
        11	public static bool Equals( string a, string b )�ж�����ָ���� string �����Ƿ������ͬ��ֵ��
        12	public static string Format( string format, Object arg0 )��ָ���ַ�����һ��������ʽ���滻Ϊָ��������ַ�����ʾ��ʽ��
        13	public int IndexOf( char value )����ָ�� Unicode �ַ��ڵ�ǰ�ַ����е�һ�γ��ֵ������������� 0 ��ʼ��
        14	public int IndexOf( string value )����ָ���ַ����ڸ�ʵ���е�һ�γ��ֵ������������� 0 ��ʼ��
        15	public int IndexOf( char value, int startIndex )����ָ�� Unicode �ַ��Ӹ��ַ�����ָ���ַ�λ�ÿ�ʼ������һ�γ��ֵ������������� 0 ��ʼ��
        16	public int IndexOf( string value, int startIndex )����ָ���ַ����Ӹ�ʵ����ָ���ַ�λ�ÿ�ʼ������һ�γ��ֵ������������� 0 ��ʼ��
        17	public int IndexOfAny( char[] anyOf )����ĳһ��ָ���� Unicode �ַ������������ַ��ڸ�ʵ���е�һ�γ��ֵ������������� 0 ��ʼ��
        18	public int IndexOfAny( char[] anyOf, int startIndex )����ĳһ��ָ���� Unicode �ַ������������ַ��Ӹ�ʵ����ָ���ַ�λ�ÿ�ʼ������һ�γ��ֵ������������� 0 ��ʼ��
        19	public string Insert( int startIndex, string value )����һ���µ��ַ��������У�ָ�����ַ����������ڵ�ǰ string �����ָ������λ�á�
        20	public static bool IsNullOrEmpty( string value )ָʾָ�����ַ����Ƿ�Ϊ null �����Ƿ�Ϊһ���յ��ַ�����
        21	public static string Join( string separator, string[] value )����һ���ַ��������е�����Ԫ�أ�ʹ��ָ���ķָ����ָ�ÿ��Ԫ�ء�
        22	public static string Join( string separator, string[] value, int startIndex, int count )����һ���ַ��������е�ָ��λ�ÿ�ʼ��ָ��Ԫ�أ�ʹ��ָ���ķָ����ָ�ÿ��Ԫ�ء�
        23	public int LastIndexOf( char value )����ָ�� Unicode �ַ��ڵ�ǰ string ���������һ�γ��ֵ�����λ�ã������� 0 ��ʼ��
        24	public int LastIndexOf( string value )����ָ���ַ����ڵ�ǰ string ���������һ�γ��ֵ�����λ�ã������� 0 ��ʼ��
        25	public string Remove( int startIndex )�Ƴ���ǰʵ���е������ַ�����ָ��λ�ÿ�ʼ��һֱ�����һ��λ��Ϊֹ���������ַ�����
        26	public string Remove( int startIndex, int count )�ӵ�ǰ�ַ�����ָ��λ�ÿ�ʼ�Ƴ�ָ���������ַ����������ַ�����
        27	public string Replace( char oldChar, char newChar )�ѵ�ǰ string �����У�����ָ���� Unicode �ַ��滻Ϊ��һ��ָ���� Unicode �ַ����������µ��ַ�����
        28	public string Replace( string oldValue, string newValue )�ѵ�ǰ string �����У�����ָ�����ַ����滻Ϊ��һ��ָ�����ַ������������µ��ַ�����
        29	public string[] Split( params char[] separator )����һ���ַ������飬������ǰ�� string �����е����ַ��������ַ�����ʹ��ָ���� Unicode �ַ������е�Ԫ�ؽ��зָ��ġ�
        30	public string[] Split( char[] separator, int count )����һ���ַ������飬������ǰ�� string �����е����ַ��������ַ�����ʹ��ָ���� Unicode �ַ������е�Ԫ�ؽ��зָ��ġ�int ����ָ��Ҫ���ص����ַ����������Ŀ��
        31	public bool StartsWith( string value )�ж��ַ���ʵ���Ŀ�ͷ�Ƿ�ƥ��ָ�����ַ�����
        32	public char[] ToCharArray()����һ�����е�ǰ string �����������ַ��� Unicode �ַ����顣
        33	public char[] ToCharArray( int startIndex, int length )����һ�����е�ǰ string �����������ַ��� Unicode �ַ����飬��ָ����������ʼ��ֱ��ָ���ĳ���Ϊֹ��
        34	public string ToLower()���ַ���ת��ΪСд�����ء�
        35	public string ToUpper()���ַ���ת��Ϊ��д�����ء�
        36	public string Trim()�Ƴ���ǰ String �����е�����ǰ���հ��ַ��ͺ��ÿհ��ַ���
         */
    }

}
