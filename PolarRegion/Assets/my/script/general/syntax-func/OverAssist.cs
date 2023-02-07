using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public static class OverAssist  {


    //======================================

    public static bool OnThenOff(ref bool state, ref bool wait)//应放在lateUpdate中执行，只支持bool类型的状态
    {//通过与lateUpdate的配合，让true只存在于一帧之中，且在update中读取时，必定是true状态
        if (wait)
        {
            state = true;
            wait = false;
        }
        else
            state = false;

        return state;//返回改动结果
    }


    //================================================================

    public static string GetName<T>(System.Linq.Expressions.Expression<System.Func<T>> memberExpression)//获取变量本身的名称
    {
        System.Linq.Expressions.MemberExpression expressionBody = (System.Linq.Expressions.MemberExpression)memberExpression.Body;
        //猜测意思是获取函数体，然后取得函数体中含有的各变量，然后取得其名称
        return expressionBody.Member.Name;
    }
    //使用：object testObj = new object();
    //string 变量名称 = InfoGet.GetName(() => testObj);
}
