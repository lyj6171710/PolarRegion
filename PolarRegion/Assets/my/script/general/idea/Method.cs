using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public static class Method
{
    public static void VaryToAndWithDo(int from, int to, Action<int> dealAdd, Action<int> dealMinus, Action<bool> dealEnd = null)
    {
        //��A�䵽B��Ȼ��Ծ���������ֵ��˳�㴦��������ֵ����A����������B
        //��ѭ��ִ�е�ί�У���ü�����������Ҫ���б𣬲�Ȼ�������ܸ�����������ֲ��add��minus
        //deal��int�������ǵ�ǰ�������������˼
        int vary = to - from;
        bool addOrMinus = vary > 0 ? true : false;

        Action<int> loop;
        if (addOrMinus)//��������
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
            oneTurn(cur);//ÿһ�����ݻ�����ֵ��û�ܷ���ֹͣҪ��ʱ
            cur += addOrMinus ? 1 : -1;
        }

        return cur - from;
    }

    public static void VaryUntilZero(int needVary, Action<int> dealWhenAdd, Action<int> dealWhenMinus, Action<bool> dealEnd = null)
    {
        //varyӦ����ͨ��to-from�õ��ģ����from�ı仯
        VaryToAndWithDo(needVary, 0, dealWhenMinus, dealWhenAdd, dealEnd);
        //����Ĳ����Ǳ仯���������������������ô���ᱻ���٣�������minus��������ͼ�Ǳ���ʱ�򣬵���add
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
    }//������������ȡ��ݹ�

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
                wait.Push(sub);//�����ϴ����Ҳ�ȡ
        }
    }//������������ȡ�ջ

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
    }//������������ȡ�����

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
    }//�Ƿ���ϣ�����Զ���

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
                wait.Push(sub);//�����ϴ����Ҳ�ȡ
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
    //��ԭ��ɢ��������ڵ㣬�ڵ��Ϊ֧��Y�ͽ��D
    //֧��ͽ��������ֻ��֧����ܼ��������ѯ�������Ҳ���ܵ��������֧��
    //����Ĳ�ѯ�����ڲ�ѯ���������Ľ��
    //���ֽṹ��һ��ʹ���龳�ǣ�֧�㱻�����ṩ������������������ʲô

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
            if (WhetherFind(super))//�ȶԱȸ������ٲ����Ӽ����ۺ���˵Ҫʡ����Щ
                return super;
            else
            {
                foreach (T sub in SubIter(super))//������Խͬ��Χͬ�㼶����
                {
                    if (sub != from)//��������
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
                    return null;//�Ӽ��Ѿ����꣬��û�и�����ļ�����
            }
        }
        else return null;
    }
    //������ң����Լ����Լ������Լ��Ľڵ㲻�ᱻ����
    //��ȷ�����������ڵ�ֻ�ᱻ�Ա�һ��
}

public class MethodRangeAccess
{
    //���Ʋ�ͬ��Χ(�����򻥳⣬�������ཻ)��ͬһ�������ʹ��

    //����ʱ��Խ��ģ�Խ����ʹ��
    //�Ѿ��������û�з���ʹ�õģ�Խ�ͺ�ʹ��

    public object holder { get; private set; }//�߼��ϵĵ�ǰ������

    List<object> mRequesters = new List<object>();//ʹ���߶���

    public bool RequestUse(object asker)
    {
        for (int i = 0; i < mRequesters.Count - 1; i++)//�����һ��������֮ǰ�����������߱Ƚ�
            if (mRequesters[i] == asker) return false;//�����ȣ��͵õȴ����������߲�����ϲ���
        holder = asker;
        if (mRequesters.Count > 0)
        {
            if (mRequesters[mRequesters.Count - 1] != asker)//���������һ��������
                mRequesters.Add(asker);
            //����Ѿ������һ������ôά��״̬���伴��
        }
        else
            mRequesters.Add(asker);
        return true;
    }
    //����ֵ����asker��ǰ�ܷ�ʹ����Դ(��Դ���������)

    public bool LeaveUse(object asker)
    {
        for (int i = 0; i < mRequesters.Count - 1; i++)//�����һ��֮ǰ�ıȽ�
            if (mRequesters[i] == asker) return false;
        if (mRequesters.Count > 0)
        {
            if (mRequesters[mRequesters.Count - 1] == asker)//�������һ��������
            {
                mRequesters.Remove(asker);//��ʹ�õ���ʹ��Ϊһ������
                holder = mRequesters.GetLast();
                return true;
            }
            else//���ǵĻ�����Ȩ����
                return false;
        }
        else
            return false;
    }
    //����ֵ����asker�Ƿ�ɹ���ʹ��̬ת��Ϊ��ȥ̬��
    //����ʱ���²�����asker����Դ������Զ���Ĺ�����иı�
}