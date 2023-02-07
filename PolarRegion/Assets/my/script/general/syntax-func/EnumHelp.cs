using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public static class ExtendEnum
{
    public static int toInt(this Enum e)
    {
        return e.GetHashCode();
        //转摘：int对象返回的Hash值就是它的数值本身，同为整型数据的Enum应该也返回它的数值才对
    }

    public static T toEnum<T>(this string str) where T : Enum
    {
        return (T)Enum.Parse(typeof(T), str, true);
        //根据字符串来获取枚举值
        //Enum.Parse()方法带3个参数，第一个参数是要使用的枚举类型，其语法是关键字typeof后跟放在括号中的枚举类名。
        //第二个参数是要转换的字符串，第三个参数是一个bool，指定在进行转换时是否忽略大小写。
        //最后，注意Enum.Parse()方法实际上返回一个对象引用，我们需要把这个对象显式转换为需要的枚举类型
    }
}

public class EnumHelp {

    enum Color
    {
        Red = 0xff0000,
        Orange = 0xFFA500,
        Yellow = 0xFFFF00,
        Lime = 0x00FF00,
        Cyan = 0x00FFFF,
        Blue = 0x0000FF,
        Purple = 0x800080
    }

    void Template(string[] args)
    {
        Color color = Color.Blue;
        string colorString = " Blue";
        int colorValue = 0x0000FF;

        // 枚举转字符串
        string enumStringOne = color.ToString(); //效率低，不推荐
        string enumStringTwo = Enum.GetName(typeof(Color), color);//推荐

        // 枚举转值
        int enumValueOne = color.GetHashCode();
        int enumValueTwo = (int)color;
        int enumValueThree = Convert.ToInt32(color);

        // 字符串转枚举
        Color enumOne = (Color)Enum.Parse(typeof(Color), colorString);

        // 字符串转值
        int enumValueFour = (int)Enum.Parse(typeof(Color), colorString);

        // 值转枚举
        Color enumTwo = (Color)colorValue;
        Color enumThree = (Color)Enum.ToObject(typeof(Color), colorValue);

        // 值转字符串
        string enumStringThree = Enum.GetName(typeof(Color), colorValue);
    }
}
