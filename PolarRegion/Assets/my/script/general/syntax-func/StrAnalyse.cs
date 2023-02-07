using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;

public static class StrAnalyse
{

    public static bool IsSupport(string[] socket, string[] plug)//前者包含后者全体时，返回真
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

    public static bool HaveSame(string[] group, string match)//前者含有后者时，返回真
    {
        foreach (string s in group)
        {
            if (s == match) return true;
        }
        return false;
    }

    public static bool HaveSame(string[] group, string[] others)//前者含有后者之一时，返回真
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

    public static int[] PickNum(string source)//从字符串中提取数字
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
        ToLower()    //转为小写字符串"AbC"-->"abc"
        ToUpper()    //转为大写"AbC" -->"ABC"
        Trim()       //去掉字符串首尾的空格"  abc "-->"abc"
        Equals(string value,StringComparison comparisonType);     //相等判断

        CompareTo(string value)             //与value比较大小

        Split(params char [] separator)     //separator 是分隔字符，如：','、'|' 等等。    
        Split(char [] separator ,StringSplitOptions  splitOpt)//StringSplitOptions.RemoveEmptyEntries  
        Split(string[] separator,StringSplitOptions splitOpt)// 按字符串分隔

        Replace(char oldChar,char newChar)  //替换字符串中的字符，如：'a'替换为'b'
        Replace(string oldStr,string newStr)//替换字符串中的字符串，如："李时珍"替换为"李秀丽"

        SubString(int startIndex)            //从指定序号开始,一直到最后,组成的字符串
        SubString(int startIndex,int length) //从指定序号startIndex,连续取length个，如果超过长度会报异常

        Contains(char c)      // 是否包含 字符
        Contains(string str)  // 是否包含 子字符串

        StartsWith(string str) //是否以str开头,如：http://baidu.com就以http://开头
        EndsWith(string str)   //是否以str结尾

        IndexOf(char c)        //找到第一个字符c的index，如果没找到返回-1
        IndexOf(string str)    //找到第一个字符串str的位置
         */

        /*
        1	public static int Compare( string strA, string strB )比较两个指定的 string 对象，并返回一个表示它们在排列顺序中相对位置的整数。该方法区分大小写。
        2	public static int Compare( string strA, string strB, bool ignoreCase )比较两个指定的 string 对象，并返回一个表示它们在排列顺序中相对位置的整数。但是，如果布尔参数为真时，该方法不区分大小写。
        3	public static string Concat( string str0, string str1 )连接两个 string 对象。
        4	public static string Concat( string str0, string str1, string str2 )连接三个 string 对象。
        5	public static string Concat( string str0, string str1, string str2, string str3 )连接四个 string 对象。
        6	public bool Contains( string value )返回一个表示指定 string 对象是否出现在字符串中的值。
        7	public static string Copy( string str )创建一个与指定字符串具有相同值的新的 String 对象。
        8	public void CopyTo( int sourceIndex, char[] destination, int destinationIndex, int count )从 string 对象的指定位置开始复制指定数量的字符到 Unicode 字符数组中的指定位置。
        9	public bool EndsWith( string value )判断 string 对象的结尾是否匹配指定的字符串。
        10	public bool Equals( string value )判断当前的 string 对象是否与指定的 string 对象具有相同的值。
        11	public static bool Equals( string a, string b )判断两个指定的 string 对象是否具有相同的值。
        12	public static string Format( string format, Object arg0 )把指定字符串中一个或多个格式项替换为指定对象的字符串表示形式。
        13	public int IndexOf( char value )返回指定 Unicode 字符在当前字符串中第一次出现的索引，索引从 0 开始。
        14	public int IndexOf( string value )返回指定字符串在该实例中第一次出现的索引，索引从 0 开始。
        15	public int IndexOf( char value, int startIndex )返回指定 Unicode 字符从该字符串中指定字符位置开始搜索第一次出现的索引，索引从 0 开始。
        16	public int IndexOf( string value, int startIndex )返回指定字符串从该实例中指定字符位置开始搜索第一次出现的索引，索引从 0 开始。
        17	public int IndexOfAny( char[] anyOf )返回某一个指定的 Unicode 字符数组中任意字符在该实例中第一次出现的索引，索引从 0 开始。
        18	public int IndexOfAny( char[] anyOf, int startIndex )返回某一个指定的 Unicode 字符数组中任意字符从该实例中指定字符位置开始搜索第一次出现的索引，索引从 0 开始。
        19	public string Insert( int startIndex, string value )返回一个新的字符串，其中，指定的字符串被插入在当前 string 对象的指定索引位置。
        20	public static bool IsNullOrEmpty( string value )指示指定的字符串是否为 null 或者是否为一个空的字符串。
        21	public static string Join( string separator, string[] value )连接一个字符串数组中的所有元素，使用指定的分隔符分隔每个元素。
        22	public static string Join( string separator, string[] value, int startIndex, int count )连接一个字符串数组中的指定位置开始的指定元素，使用指定的分隔符分隔每个元素。
        23	public int LastIndexOf( char value )返回指定 Unicode 字符在当前 string 对象中最后一次出现的索引位置，索引从 0 开始。
        24	public int LastIndexOf( string value )返回指定字符串在当前 string 对象中最后一次出现的索引位置，索引从 0 开始。
        25	public string Remove( int startIndex )移除当前实例中的所有字符，从指定位置开始，一直到最后一个位置为止，并返回字符串。
        26	public string Remove( int startIndex, int count )从当前字符串的指定位置开始移除指定数量的字符，并返回字符串。
        27	public string Replace( char oldChar, char newChar )把当前 string 对象中，所有指定的 Unicode 字符替换为另一个指定的 Unicode 字符，并返回新的字符串。
        28	public string Replace( string oldValue, string newValue )把当前 string 对象中，所有指定的字符串替换为另一个指定的字符串，并返回新的字符串。
        29	public string[] Split( params char[] separator )返回一个字符串数组，包含当前的 string 对象中的子字符串，子字符串是使用指定的 Unicode 字符数组中的元素进行分隔的。
        30	public string[] Split( char[] separator, int count )返回一个字符串数组，包含当前的 string 对象中的子字符串，子字符串是使用指定的 Unicode 字符数组中的元素进行分隔的。int 参数指定要返回的子字符串的最大数目。
        31	public bool StartsWith( string value )判断字符串实例的开头是否匹配指定的字符串。
        32	public char[] ToCharArray()返回一个带有当前 string 对象中所有字符的 Unicode 字符数组。
        33	public char[] ToCharArray( int startIndex, int length )返回一个带有当前 string 对象中所有字符的 Unicode 字符数组，从指定的索引开始，直到指定的长度为止。
        34	public string ToLower()把字符串转换为小写并返回。
        35	public string ToUpper()把字符串转换为大写并返回。
        36	public string Trim()移除当前 String 对象中的所有前导空白字符和后置空白字符。
         */
    }

}
