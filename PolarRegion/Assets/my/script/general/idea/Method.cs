using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public static class Method
{
    public static void VaryToAndWithDo(int from, int to, Action<int> dealAdd, Action<int> dealMinus, Action<bool> dealEnd = null)
    {
        //从A变到B，然后对经过的所有值做顺便处理，经过的值包括A，但不包括B
        //被循环执行的委托，最好减少其内容需要的判别，不然增加性能负担，所以这分拆成add和minus
        //deal的int参数，是当前到达了哪里的意思
        int vary = to - from;
        bool addOrMinus = vary > 0 ? true : false;

        Action<int> loop;
        if (addOrMinus)//轮流方向
            loop = (curAt) => { dealAdd(curAt); vary--; };
        else
            loop = (curAt) => { dealMinus(curAt); vary++; };

        while (vary != 0) loop(to - vary);

        if (dealEnd != null) dealEnd(addOrMinus);
    }

    public static int VaryUntilReach(int from, bool addOrMinus, Func<int, bool> ifReach, Action<int> oneTurn)
    {
        int cur = from;
        while (!ifReach(cur))
        {
            oneTurn(cur);//每一次所递换到的值，没能符合停止要求时
            cur += addOrMinus ? 1 : -1;
        }

        return cur - from;
    }

    public static void VaryUntilZero(int needVary, Action<int> dealWhenAdd, Action<int> dealWhenMinus, Action<bool> dealEnd = null)
    {
        //vary应该是通过to-from得到的，相对from的变化
        VaryToAndWithDo(needVary, 0, dealWhenMinus, dealWhenAdd, dealEnd);
        //这里的参数是变化量，所以如果是正数，那么将会被减少，而调用minus，但是意图是变多的时候，调用add
    }

    //===========================================

    public static void TraverseRecurDF<T>(
        T from,
        Func<T,IEnumerable<T>> SubIter,
        Action<T> SelfInterview)
    {
        SelfInterview(from);

        foreach (T sub in SubIter(from))
        {
            TraverseRecurDF(sub, SubIter, SelfInterview);
        }
    }//遍历、深度优先、递归

    public static void TraverseStackDF<T>(
        T from,
        Func<T,IEnumerable<T>> SubIter,
        Action<T> SelfInterview)
    {
        Stack<T> wait = new Stack<T>();
        wait.Push(from);
        while (wait.Count > 0)
        {
            T cur = wait.Pop();
            SelfInterview(cur);
            foreach (T sub in SubIter(cur))
                wait.Push(sub);//将不断从最右侧取
        }
    }//遍历、深度优先、栈

    public static void TraverseQueueBF<T>(
        T from,
        Func<T,IEnumerable<T>> SubIter,
        Action<T> SelfInterview)
    {
        Queue<T> wait = new Queue<T>();
        wait.Enqueue(from);
        while (wait.Count > 0)
        {
            T cur = wait.Dequeue();
            SelfInterview(cur);
            foreach (T sub in SubIter(cur))
                wait.Enqueue(sub);
        }
    }//遍历、广度优先、队列

    //===========================================

    public static bool FindInRecurDF<T>(
        T from,
        Func<T, IEnumerable<T>> SubIter,
        Func<T, bool> WhetherFind,
        out T result)
    {
        if (WhetherFind(from))
        {
            result = from;
            return true;
        }

        foreach (T sub in SubIter(from))
        {
            if (FindInRecurDF(sub, SubIter, WhetherFind, out result))
                return true;
        }

        result = default(T);
        return false;
    }//是否符合，外界自定义

    public static bool FindInStackDF<T>(
        T from,
        Func<T,IEnumerable<T>> SubIter,
        Func<T,bool> WhetherFind,
        out T result)
    {
        Stack<T> wait = new Stack<T>();
        wait.Push(from);
        while (wait.Count > 0)
        {
            T cur = wait.Pop();
            if (WhetherFind(cur))
            {
                result = cur;
                return true;
            }
            foreach (T sub in SubIter(cur))
                wait.Push(sub);//将不断从最右侧取
        }
        result = default(T);
        return false;
    }

    public static bool FindInQueueBF<T>(
        T from,
        Func<T,IEnumerable<T>> SubIter,
        Func<T,bool> WhetherFind,
        out T result)
    {
        Queue<T> wait = new Queue<T>();
        wait.Enqueue(from);
        while (wait.Count > 0)
        {
            T cur = wait.Dequeue();
            if (WhetherFind(cur))
            {
                result = cur;
                return true;
            }
            foreach (T sub in SubIter(cur))
                wait.Enqueue(sub);
        }
        result = default(T);
        return false;
    }

    //=========================================

    public static bool FindInStackDF<D,Y>(
        Y from,
        Func<Y, IEnumerable<Y>> SubPackIter,
        Func<Y, IEnumerable<D>> SubItemIter,
        Func<Y, D, bool> WhetherFind,
        out D result)
    {
        Stack<Y> wait = new Stack<Y>();
        wait.Push(from);
        while (wait.Count > 0)
        {
            Y cur = wait.Pop();
            foreach (D item in SubItemIter(cur))
            {
                if (WhetherFind(cur, item)) 
                {
                    result = item;
                    return true;
                }
            }
            foreach (Y sub in SubPackIter(cur))
                wait.Push(sub);
        }
        result = default(D);
        return false;
    }
    //从原点散发出多个节点，节点分为支点Y和结点D
    //支点和结点有区别，只有支点才能继续延申查询，而结点也不能当作特殊的支点
    //这里的查询，用于查询符合条件的结点
    //这种结构，一般使用情境是，支点被用来提供线索，而不真正承载什么

    //==========================================

    public static T ReverseFindInRecur<T>(
        T from,
        Func<T, T> ItSuper,
        Func<T, IEnumerable<T>> SubIter,
        Func<T, bool> WhetherFind) where T : class
    {
        T super = ItSuper(from);
        if (super != null)
        {
            if (WhetherFind(super))//先对比父级，再查它子级，综合来说要省性能些
                return super;
            else
            {
                foreach (T sub in SubIter(super))//优先在越同范围同层级中找
                {
                    if (sub != from)//可以忍受
                    {
                        T result;
                        if (FindInQueueBF(sub, SubIter, WhetherFind, out result))
                            return result;
                    }
                }
                super = ItSuper(super);
                if (super != null)
                    return ReverseFindInRecur(super, ItSuper, SubIter, WhetherFind);
                else
                    return null;//子级已经查完，又没有更上面的级别了
            }
        }
        else return null;
    }
    //反向查找，但自己，以及附属自己的节点不会被查找
    //能确保所有其它节点只会被对比一次
}

public class MethodRangeAccess
{
    //控制不同范围(包含或互斥，不存在相交)对同一个事物的使用

    //申请时机越后的，越优先使用
    //已经申请过且没有放弃使用的，越滞后使用

    public object holder { get; private set; }//逻辑上的当前享用者

    List<object> mRequesters = new List<object>();//使用者队列

    public bool RequestUse(object asker)
    {
        for (int i = 0; i < mRequesters.Count - 1; i++)//与最后一个申请者之前的所有申请者比较
            if (mRequesters[i] == asker) return false;//如果相等，就得等待后面申请者操作完毕才行
        holder = asker;
        if (mRequesters.Count > 0)
        {
            if (mRequesters[mRequesters.Count - 1] != asker)//它不是最后一个申请者
                mRequesters.Add(asker);
            //如果已经是最后一个，那么维持状态不变即可
        }
        else
            mRequesters.Add(asker);
        return true;
    }
    //返回值代表asker当前能否使用资源(资源是外界管理的)

    public bool LeaveUse(object asker)
    {
        for (int i = 0; i < mRequesters.Count - 1; i++)//与最后一个之前的比较
            if (mRequesters[i] == asker) return false;
        if (mRequesters.Count > 0)
        {
            if (mRequesters[mRequesters.Count - 1] == asker)//它是最后一个申请者
            {
                mRequesters.Remove(asker);//从使用到不使用为一个周期
                holder = mRequesters.GetLast();
                return true;
            }
            else//不是的话，无权操作
                return false;
        }
        else
            return false;
    }
    //返回值代表asker是否成功从使用态转变为离去态，
    //这种时机下才允许asker对资源按外界自定义的规则进行改变
}