using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalData : MonoBehaviour
{
    /*待写入：
    public class Temp_paras//临时参数
    {//所记录者都是将要被用到的参数，直到该参数被使用且被消耗掉
        Dictionary<string, float> 浮点变量;
        Dictionary<string, int> 整型变量;
        Dictionary<string, string> 字符串变量;
        Dictionary<string, bool> 布尔变量;

        public void cast_(string name,float value)
        {
            浮点变量.Add(name, value);
        }
        public void cast_(string name, int value)
        {
            整型变量.Add(name, value);
        }
        public void cast_(string name, string value)
        {
            字符串变量.Add(name, value);
        }
        public void cast_(string name, bool value)
        {
            布尔变量.Add(name, value);
        }

        public bool get_(string name,ref float value)
        {
            if (浮点变量.ContainsKey(name))
            {
                value = 浮点变量[name];
                浮点变量.Remove(name);
                return true;
            }
            else
                return false;
        }
        public bool get_(string name, ref int value)
        {
            if (浮点变量.ContainsKey(name))
            {
                value = 整型变量[name];
                浮点变量.Remove(name);
                return true;
            }
            else
                return false;
        }
        public bool get_(string name, ref string value)
        {
            if (浮点变量.ContainsKey(name))
            {
                value = 字符串变量[name];
                浮点变量.Remove(name);
                return true;
            }
            else
                return false;
        }
        public bool get_(string name, ref bool value)
        {
            if (浮点变量.ContainsKey(name))
            {
                value = 布尔变量[name];
                浮点变量.Remove(name);
                return true;
            }
            else
                return false;
        }
    }
    */
}
