using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RegexHelp
{
    public const string StrPtnFmVec2Int = @"\([0-9]+, [0-9]+\)";

    public static string Wrap(string regex) => "^" + regex + "$";

    public static string StrPtnGetFileNameInFullPath = @"\[^\]*$";//待检查
    //[^\]:表示不能出现\
    //*表示前面的字符可以出现任意次
    //$表示匹配的字符串，应该在末尾
}
