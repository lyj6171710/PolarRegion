using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RegexHelp
{
    public const string StrPtnFmVec2Int = @"\([0-9]+, [0-9]+\)";

    public static string Wrap(string regex) => "^" + regex + "$";

    public static string StrPtnGetFileNameInFullPath = @"\[^\]*$";//�����
    //[^\]:��ʾ���ܳ���\
    //*��ʾǰ����ַ����Գ��������
    //$��ʾƥ����ַ�����Ӧ����ĩβ
}
